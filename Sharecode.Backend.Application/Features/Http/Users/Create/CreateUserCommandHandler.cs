using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.MetaKeys;
using Sharecode.Backend.Utilities.SecurityClient;

namespace Sharecode.Backend.Application.Features.Http.Users.Create;

internal class CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository, ITokenClient tokenClient, ISecurityClient securityClient)
    : IRequestHandler<CreateUserCommand, UserCreatedResponse>
{
    public async Task<UserCreatedResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        
        securityClient.CreatePasswordHash(command.Password!, out var passwordHash, out var salt);
        User user = new User()
        {
            EmailAddress = command.EmailAddress!,
            FirstName = command.FirstName!,
            LastName = command.LastName!,
            MiddleName = command.MiddleName,
            Visibility = AccountVisibility.Public,
            Salt = salt,
            PasswordHash = passwordHash,
            ProfilePicture = command.ProfilePicture
        };
        user.SetActive();
        user.AccountSetting = new AccountSetting
        {
            UserId = user.Id,
            User = user,
            Id = Guid.NewGuid()
        };
        user.SetMeta(MetaKeys.UserKeys.RecentlyVisitedSnippets, new List<Guid>());
        user.SetUserPermissions();
        userRepository.Register(user);
        await unitOfWork.CommitAsync(cancellationToken);
        return UserCreatedResponse.From(user);
    }
}