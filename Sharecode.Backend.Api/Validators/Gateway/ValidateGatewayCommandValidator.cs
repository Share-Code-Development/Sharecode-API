using FluentValidation;
using Sharecode.Backend.Application.Features.Gateway.Validate;

namespace Sharecode.Backend.Api.Validators.Gateway;

public class ValidateGatewayCommandValidator : AbstractValidator<ValidateGatewayCommand>
{
    public ValidateGatewayCommandValidator()
    {

    }
}