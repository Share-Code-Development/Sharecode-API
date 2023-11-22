using FluentValidation;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Users.Create;

public class CreateUserCommandPersistenceValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandPersistenceValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.EmailAddress)
            .MustAsync(async (email, token) => await userRepository.IsEmailAddressUnique(email, token))
            .WithErrorCode("020021")
            .WithMessage("This email address has already been registered");
    }
}