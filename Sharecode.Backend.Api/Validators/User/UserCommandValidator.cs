using FluentValidation;
using Sharecode.Backend.Application.Features.Users.Create;
using Sharecode.Backend.Application.Features.Users.ForgotPassword;
using Sharecode.Backend.Application.Features.Users.Login;
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

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address should be present");
    }
}

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