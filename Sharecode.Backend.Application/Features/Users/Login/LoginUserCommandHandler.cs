using Google.Apis.Auth;
using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.KeyValue;
using Sharecode.Backend.Utilities.SecurityClient;

namespace Sharecode.Backend.Application.Features.Users.Login;

public class LoginUserCommandHandler(IUserService userService, IUnitOfWork unitOfWork, IUserRepository userRepository, ISecurityClient securityClient, IGatewayService gatewayService, ITokenClient tokenClient, IRefreshTokenRepository refreshTokenRepository, Namespace cloudFlareKeyValue)
    : IRequestHandler<LoginUserRequest, LoginUserResponse>
{
    public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        User user = await GetOrRegisterUser(request, cancellationToken);
        
        if (!user.Active)
        {
            throw new AccountLockedException(user.EmailAddress, user.InActiveReason!);
        }
        
        user.SetLastLogin();
        var accessCredentials = tokenClient.Generate(user);
        await refreshTokenRepository.AddAsync(accessCredentials.UserRefreshToken, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);
        return LoginUserResponse.From(user, accessCredentials);
    }

    private async Task<User> GetOrRegisterUser(LoginUserRequest request, CancellationToken token = default)
    {
        if (request.Type == AuthorizationType.Regular)
        {
            return await FromRegular(request, token);
        }
        else
        {
            return await FromGoogle(request, token);
        }
    }

    private async Task<User> FromGoogle(LoginUserRequest request, CancellationToken token = default)
    {
        string? googleClientId = cloudFlareKeyValue.Of(KeyVaultConstants.GoogleClientId)?.Value;
        if (string.IsNullOrEmpty(googleClientId))
            throw new ApplicationException("Failed to fetch google auth");

        GoogleJsonWebSignature.Payload? payload = null;
        GoogleJsonWebSignature.ValidationSettings settings = new();
        
        settings.Audience = new List<string>() { googleClientId };
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
        }
        catch (Exception e)
        {
            throw new InvalidAuthFromSocialException(AuthorizationType.Google, "Authentication failed due to invalid token");
        }

        if (payload == null)
        {
            throw new InvalidAuthFromSocialException(AuthorizationType.Google, "Authentication failed due to invalid token");
        }

        string emailAddress = payload.Email;
        User? user = await userRepository.GetUserByEmailIncludingAccountSettings(emailAddress, true, token);
        if (user == null)
        {
            user = await RegisterFromGoogle(payload, token);
        }
        else
        {
            user.ProfilePicture = payload.Picture;
            var names = GetNameFromGooglePayload(payload);
            user.FirstName = names.Item1;
            user.MiddleName = names.Item2;
            user.LastName = names.Item3;
        }

        return user;
    }

    private async Task<User> RegisterFromGoogle(GoogleJsonWebSignature.Payload payload, CancellationToken token = default)
    {
        string emailAddress = payload.Email;
        var names = GetNameFromGooglePayload(payload);
        var profilePicture = payload.Picture;
        AccountSetting setting = new AccountSetting();
        var user = new User()
        {
            Id = Guid.NewGuid(),
            EmailAddress = emailAddress,
            FirstName = names.Item1,
            MiddleName = names.Item2,
            LastName = names.Item3,
            Visibility = AccountVisibility.Private,
            AccountSetting = setting,
            ProfilePicture = profilePicture,
            PasswordHash = null,
            Salt = null,
        };
        setting.User = user;
        setting.UserId = user.Id;
        user.VerifyUser();
        user.SetActive();
        await userRepository.AddAsync(user, token);
        return user;
    }
    
    private async Task<User> FromRegular(LoginUserRequest request, CancellationToken token = default)
    {
        var user = await userRepository.GetUserByEmailIncludingAccountSettings(request.EmailAddress!, true, token);
        if (user == null)
            throw new EntityNotFoundException(typeof(User), request.EmailAddress!);
        
        if (user.Salt == null || user.PasswordHash == null)
        {
            throw new PasswordNotYetGeneratedException();
        }

        var isPasswordMatching = securityClient.VerifyPasswordHash(request.Password!, user.PasswordHash!, user.Salt!);
        if (!isPasswordMatching)
        {
            throw new InvalidPasswordException();
        }

        if (!user.EmailVerified)
        {
            var limitReached = await gatewayService.IsLimitReachedAsync(user.Id, GatewayRequestType.VerifyUserAccount, token);
            if (limitReached)
            {
                throw new EmailNotVerifiedException(user.EmailAddress, "Please check your email for pending verifications!");
            }

            user.ResendEmailVerification();
            throw new EmailNotVerifiedException(user.EmailAddress, "We have send a verification email to your address, Please check your email");
        }
        
        return user;
    }

    private (string, string?, string) GetNameFromGooglePayload(GoogleJsonWebSignature.Payload payload)
    {
        string payloadName = payload.Name;
        string[] split = payload.Name.Split(" ");
        if (split.Length >= 3)
        {
            return new (split[0], split[1], split[2]);
        }else if (split.Length == 2)
        {
            return new(split[0], null, split[1]);
        }
        else
        {
            return new(split[0], null, "Unknown");
        }
    }
}