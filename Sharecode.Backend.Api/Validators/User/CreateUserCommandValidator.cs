using FluentValidation;
using Sharecode.Backend.Application.Features.Users.Create;

namespace Sharecode.Backend.Api.Validators.User;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.EmailAddress)
            .EmailAddress()
            .WithMessage("Email address is malformed");
    }
}