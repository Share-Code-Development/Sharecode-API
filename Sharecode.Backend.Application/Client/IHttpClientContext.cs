using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Client;

public interface IHttpClientContext
{
    bool IsApiRequest { get; }
    string? EmailAddress { get; }
    Task<Guid?> GetUserIdentifierAsync();
    Task<User?> GetNonTrackingUserAsync();
    string CacheKey { get; protected set; }
    bool HasCacheKey { get;  }
    string[] CacheKeyBlock { get; set; }
    Dictionary<string, HashSet<string>> CacheInvalidRecords { get; }
    bool HasPermission(Permission key);
    Task<bool> HasPermissionAsync(Permission key, CancellationToken token = default);
    void AddCacheKeyToInvalidate(string module, params string[] keys);
}