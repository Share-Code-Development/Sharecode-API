using Sharecode.Backend.Domain.Entity.Gateway;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Service;

public interface IGatewayService
{
    Task<bool> IsLimitReachedAsync(Guid sourceId, GatewayRequestType type, CancellationToken token = default);

    Task<GatewayRequest?> CreateGatewayRequestAsync(Guid sourceId, GatewayRequestType requestType,
        bool overrideLimit = false, DateTime? expiry = null, CancellationToken token = default);
}