using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Gateway.Validate;

public class ValidateGatewayAppRequest : IAppRequest<ValidateGatewayCommandResponse>
{
    public Guid GatewayId { get; set; }
    public GatewayRequestType Type { get; set; }
    
    public string NewPassword { get; set; }
}