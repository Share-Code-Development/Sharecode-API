using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Client;

public interface IHttpClientContext
{
    bool IsApiRequest { get; }
    string? EmailAddress { get; }
    Task<Guid?> GetUserIdentifierAsync();
    Task<User?> GetNonTrackingUserAsync();
    string CacheKey { get; set; }
    bool HasCacheKey { get;  }
    bool HasPermission(Permission key);
    Task<bool> HasPermissionAsync(Permission key, CancellationToken token = default);
}