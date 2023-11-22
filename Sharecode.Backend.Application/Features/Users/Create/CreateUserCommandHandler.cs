using MediatR;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Users.Create;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserCreatedResponse>
{
    //Inject Repo
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenClient _tokenClient;
    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, ITokenClient tokenClient)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenClient = tokenClient;
    }

    public async Task<UserCreatedResponse> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User()
        {
            EmailAddress = command.EmailAddress,
            FirstName = command.FirstName,
            MiddleName = command.MiddleName,
            LastName = command.LastName,
            Salt = command.Salt,
            PasswordHash = command.PasswordHash,
            Visibility = AccountVisibility.Private,
        };
        user.AccountSetting = new()
        {
            User = user 
        };

        _userRepository.Register(user);
        AccessCredentials accessCredentials = _tokenClient.Generate(user);
        _refreshTokenRepository.Add(accessCredentials.UserRefreshToken);
        
        await _unitOfWork.CommitAsync(cancellationToken);
        
        return UserCreatedResponse.From(user, accessCredentials);
    }
}