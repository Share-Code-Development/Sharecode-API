using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Domain.Entity.Gateway;

public class GatewayRequest : BaseEntity
{
    public GatewayRequestType RequestType { get; private set; }
    public Guid SourceId { get; private set; }
    public DateTime Expiry { get; private set; }
    public bool IsValid { get; private set; } = true;
    public bool IsCompleted { get; private set; } = false;
    public DateTime? ProcessedAt { get; private set; }

    public static GatewayRequest CreateRequest(GatewayRequestType requestType, Guid sourceId)
    {
        var expiry = GetExpiry(requestType);
        return new GatewayRequest()
        {
            RequestType = requestType,
            SourceId = sourceId,
            Expiry = expiry
        };
    }

    public void SetProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        IsCompleted = true;
    }

    #region Private

    private static DateTime GetExpiry(GatewayRequestType requestType)
    {
        return requestType switch
        {
            GatewayRequestType.VerifyUserAccount => DateTime.UtcNow.AddMinutes(10),
            _ => DateTime.UtcNow
        };
    }
    #endregion
    
}