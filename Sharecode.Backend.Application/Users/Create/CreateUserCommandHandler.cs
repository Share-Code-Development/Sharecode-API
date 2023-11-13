using MediatR;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Users.Create;

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

    public async Task<UserCreatedResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User()
        {
            EmailAddress = request.EmailAddress,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Salt = request.Salt,
            PasswordHash = request.PasswordHash,
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