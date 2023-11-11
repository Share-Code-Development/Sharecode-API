using MediatR;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Users.Create;

internal class CreateUserCommandHandler : IRequestHandler<CreateUserCommand>
{
    //Inject Repo
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
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
        await _unitOfWork.CommitAsync();
    }
}