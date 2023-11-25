using StackExchange.Redis;

namespace Sharecode.Backend.Utilities.RedisCache;

public interface IAppCacheClient
{
    TimeSpan DefaultCachingLife { get; }
    Task<bool> WriteCacheAsync(string key, object value, TimeSpan? ttl = null ,CancellationToken token = default);
    Task<string?> GetCacheAsync(string key, bool useSlidingExpiration = false, CancellationToken token = default);
    Task DeleteCacheAsync(string key,  CancellationToken token = default);
    Task DeleteCachesAsync(string[] keys, CancellationToken token = default);
    Task<int> DeleteMatchingKeysAsync(List<string> patterns, CancellationToken token = default);  
}