using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Gateway.Validate;

public record ValidateGatewayCommand(Guid GatewayId) : ICommand<ValidateGatewayCommandResponse>;