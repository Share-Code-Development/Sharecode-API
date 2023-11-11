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

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    //Inject Repo
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User()
        {
            EmailAddress = request.EmailAddress,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Salt = request.Salt,
            PasswordHash = request.PasswordHash,
            Visibility = AccountVisibility.Private
        };
        
        _userRepository.Register(user);
        AccessCredentials accessCredentials = _tokenService.Generate(user);
        _refreshTokenRepository.Add(accessCredentials.UserRefreshToken);
        
        Console.WriteLine(JsonConvert.SerializeObject(accessCredentials));
        
        await _unitOfWork.CommitAsync(cancellationToken);
        
    }
}