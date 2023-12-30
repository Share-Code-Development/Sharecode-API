using System.Diagnostics.CodeAnalysis;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Utilities.RequestDetail;

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
    Task<bool> HasPermissionAsync(Permission key, CancellationToken token = default);
    Task<bool> HasPermissionAnyAsync(CancellationToken token = default, params Permission[] key);
    Task<bool> HasPermissionAnyAsync(Permission[] key, CancellationToken token = default);
    Task<bool> HasPermissionAllAsync(CancellationToken token = default, params Permission[] key);
    Task<bool> HasPermissionAllAsync(Permission[] key,CancellationToken token = default);
    void AddCacheKeyToInvalidate(string module, params string[] keys);
    bool TryGetHeader(string key, [MaybeNullWhen(false)] out string headerValue);
    IRequestDetail RequestDetail { get; }
    bool HasAuthorizationBearer { get; }
}