using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Gateway;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Utilities.Configuration;

namespace Sharecode.Backend.Infrastructure.Service;

public class GatewayService : IGatewayService
{
    private static readonly Dictionary<GatewayRequestType, int> Limits = new Dictionary<GatewayRequestType, int>();
    private readonly ShareCodeDbContext _dbContext;
    private readonly GatewayLimitConfiguration _gatewayLimitConfiguration;
    private readonly ILogger<GatewayService> _logger;

    public GatewayService(ShareCodeDbContext dbContext, IOptions<GatewayLimitConfiguration> gatewayLimitConfiguration, ILogger<GatewayService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
        _gatewayLimitConfiguration = gatewayLimitConfiguration.Value;
    }

    public async Task<bool> IsLimitReachedAsync(Guid sourceId, GatewayRequestType type, CancellationToken token = default)
    {
        //Get the count of requests of a source where
        // 1) its of the same request type
        // 2) the request is still valid
        // 3) the request is not deleted
        // 4) the request is not expired
        // 5) the request is not completed
        // 6) the request is not processed
        var currentCount = await _dbContext.Set<GatewayRequest>()
            .CountAsync(x => 
                x.SourceId == sourceId && //Match the source id
                x.RequestType == type &&  //Match the requestType
                x.IsValid && //Match whether for some reason the gateway is set to invalid
                !x.IsDeleted && //Match whether the gateway request is deleted
                x.Expiry < DateTime.UtcNow && //It is not expired
                x.ProcessedAt == null && //Its still not processed
                !x.IsCompleted //Not completed
                , token);

        var limit = GetLimit(type, _gatewayLimitConfiguration);
        if (!limit.HasValue)
        {
            _logger.LogCritical($"Failed to get the limit of {type.ToString()} from configuration");
            return true;
        }

        return currentCount >= limit;
    }

    private int? GetLimit(GatewayRequestType requestType, GatewayLimitConfiguration config)
    {
        //Check if its in the in memory cache
        if (Limits.TryGetValue(requestType, out int val))
        {
            return val;
        }
        
        //Fetch from configuration
        string propertyName = requestType.ToString();
        PropertyInfo? propertyInfo = typeof(GatewayLimitConfiguration).GetProperty(propertyName);
        if (propertyInfo != null)
        { 
            object? value = propertyInfo.GetValue(config);
            if (value is int intValue)
            {
                //Store it in the cache
                Limits[requestType] = intValue;
                return intValue;
            }
        }
        
        return null;
    }
}