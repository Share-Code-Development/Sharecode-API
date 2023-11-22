using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Gateway.Validate;

public record ValidateGatewayAppRequest(Guid GatewayId) : IAppRequest<ValidateGatewayCommandResponse>;