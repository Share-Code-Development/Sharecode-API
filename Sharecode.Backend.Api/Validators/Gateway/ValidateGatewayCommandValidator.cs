using FluentValidation;
using Sharecode.Backend.Application.Features.Gateway.Validate;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Api.Validators.Gateway;

public class ValidateGatewayCommandValidator : AbstractValidator<ValidateGatewayAppRequest>
{
    public ValidateGatewayCommandValidator()
    {
        RuleFor(x => x.GatewayId)
            .NotEmpty()
            .WithMessage($"Should provide a valid gateway identifier");

        RuleFor(x => x.Type)
            .NotNull()
            .WithMessage($"Unknown gateway type has been provided");

        #region ResetPassword

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .When(x => x.Type == GatewayRequestType.ForgotPassword)
            .WithMessage($"Please provide a valid new password");

        RuleFor(x => x.NewPassword)
            .Null()
            .When(x => x.Type != GatewayRequestType.ForgotPassword)
            .WithMessage($"Provided restricted key-value for validation");

        #endregion


    }
}