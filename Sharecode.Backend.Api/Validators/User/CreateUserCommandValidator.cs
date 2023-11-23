using FluentValidation;
using Sharecode.Backend.Application.Features.Users.Create;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Api.Validators.User;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.FirstName).NotEmpty();

        RuleFor(x => x.LastName).NotEmpty();

        RuleFor(x => x.Password).NotNull();
    }
}