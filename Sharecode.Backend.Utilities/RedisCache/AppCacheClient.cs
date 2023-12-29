using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Utilities.RedisCache;

public class AppCacheClient([FromKeyedServices(DiKeyedServiceConstants.RedisForCache)]ConfigurationOptions connectionConfiguration, ILogger logger) : IAppCacheClient
{
    private ConnectionMultiplexer? Redis { get; set; }
    private static readonly object Lock = new();
    private readonly int _maxRetryCount = 3;
    private readonly TimeSpan _initialRetryDelay = TimeSpan.FromSeconds(2); 
    public TimeSpan DefaultCachingLife => TimeSpan.FromMinutes(3);
    
    public async Task<bool> WriteCacheAsync(string key, object? value, TimeSpan? ttl = null, CancellationToken token = default)
    {
        try
        {
            ttl ??= DefaultCachingLife;
            if (value == null)
                return false;

            string serializedValue = null;
            if (value is string s)
            {
                serializedValue = s;
            }
            else
            {
                serializedValue = JsonConvert.SerializeObject(value);
            }
            return await Database.StringSetAsync(key, serializedValue, ttl);
        }
        catch (Exception e)
        {
            logger.Error(e, $"Failed to set the {key} to redis cache");
            return false;
        }
    }
    
    public async Task<string?> GetCacheAsync(string key,bool useSlidingExpiration = false,  CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();
            RedisValue value = await Database.StringGetAsync(key);

            if (useSlidingExpiration && value.HasValue)
            {
                await Database.KeyExpireAsync(key, DefaultCachingLife);
            }

            return value.HasValue ? value.ToString() : null;
        }
        catch (OperationCanceledException)
        {
            logger.Warning("Operation was cancelled.");
            return null;
        }
        catch (Exception e)
        {
            logger.Error(e, "Error retrieving key: {Key} from cache", key);
            throw;
        }

    }
    
    public async Task DeleteCacheAsync(string key, CancellationToken token = default)
    {
        try
        {
            await Database.KeyDeleteAsync(key);
        }
        catch (Exception e)
        {
            logger.Error( e, $"Failed to delete the {key} from redis cache");
        }
    }

    public async Task DeleteCachesAsync(string[] keys, CancellationToken token = default)
    {
        try
        {
            var redisKeys = keys.Select(k => (RedisKey)k).ToArray();
            await Database.KeyDeleteAsync(redisKeys);
        }
        catch (Exception e)
        {
            logger.Error( e, "Failed to delete multiple keys from redis cache");
        }
    }

    public async Task<int> DeleteMatchingKeysAsync(List<string> patterns, CancellationToken token = default)
    {
        int deletedCount = 0;
        try
        {
            foreach (var pattern in patterns)
            {
                long cursor = 0;
                do
                {
                    RedisResult scanResult = await Database.ExecuteAsync("SCAN", cursor.ToString(), "MATCH", pattern);
                    var innerResult = (RedisResult[])scanResult;
                    cursor = long.Parse((string)innerResult[0]);

                    RedisKey[] keysToDelete = (RedisKey[])innerResult[1];
                    foreach (var key in keysToDelete)
                    {
                        bool deleted = await Database.KeyDeleteAsync(key);
                        if (deleted) deletedCount++;
                    }

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                } while (cursor != 0);
            }
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to delete keys with patterns {Key}", string.Join(",", patterns));
        }
        return deletedCount;
    }


    protected IDatabase Database
    {
        get
        {
            if (Redis is { IsConnected: true })
            {
                return Redis.GetDatabase();
            }

            lock (Lock)
            {
                //Double checking whether the connection got reestablished or not
                if (Redis is { IsConnected: true })
                {
                    return Redis.GetDatabase();
                }

                for (int retry = 0; retry < _maxRetryCount; retry++)
                {
                    try
                    {
                        if (Redis != null)
                        {
                            Redis.Dispose();
                        }
                        Redis = ConnectionMultiplexer.Connect(connectionConfiguration);
                        return Redis.GetDatabase();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Failed to reconnect to Redis-[{Type}]. Attempt {AttemptNumber}", retry + 1, DiKeyedServiceConstants.RedisForCache);
                        Thread.Sleep(_initialRetryDelay * (int)Math.Pow(2, retry));
                    }
                }

                throw new ApplicationException($"Failed to reconnect to Redis-[{DiKeyedServiceConstants.RedisForCache}] after multiple attempts.");
            }
        }
    }
}