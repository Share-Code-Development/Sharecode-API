using System.Net;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.KeyValue;
using Sharecode.Backend.Utilities.RedisCache;
using StackExchange.Redis;

namespace Sharecode.Backend.Api.Extensions;

public static class BootstrapExtensions
{
    public static IServiceCollection AddRedisClient(this IServiceCollection service, Namespace keyValueClient)
    {
        string redisConnectionString = keyValueClient.Of(KeyVaultConstants.RedisConnectionString)?.Value ?? string.Empty;
        string redisUserName = keyValueClient.Of(KeyVaultConstants.RedisConnectionUserName)?.Value ?? string.Empty;
        string redisPassword = keyValueClient.Of(KeyVaultConstants.RedisConnectionPassword)?.Value ?? string.Empty;
        
        string[]? strings = redisConnectionString.Split(":");
        DnsEndPoint endPoint = new DnsEndPoint(strings[0], Convert.ToInt32(strings[1]));
        var configurationOption = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            User = redisUserName,
            Password = redisPassword,
            EndPoints = {endPoint}
        };

        service.AddSingleton<ConfigurationOptions>(configurationOption);
        service.AddSingleton<IAppCacheClient, AppCacheClient>();
        return service;
    } 
}