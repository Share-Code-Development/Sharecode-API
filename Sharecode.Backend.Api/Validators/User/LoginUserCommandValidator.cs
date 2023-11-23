using FluentValidation;
using Sharecode.Backend.Application.Features.Users.Login;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Api.Validators.User;

public class LoginUserCommandValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotNull()
            .EmailAddress()
            .When(x => x.Type == AuthorizationType.Regular);

        RuleFor(x => x.Password)
            .NotNull()
            .When(x => x.Type == AuthorizationType.Regular);

        RuleFor(x => x.IdToken)
            .NotNull()
            .When(x => x.Type == AuthorizationType.Google);
    }
}