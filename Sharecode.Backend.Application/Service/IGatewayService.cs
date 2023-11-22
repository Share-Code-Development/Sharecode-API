using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Service;

public interface IGatewayService
{
    Task<bool> IsLimitReachedAsync(Guid sourceId, GatewayRequestType type, CancellationToken token = default);
}