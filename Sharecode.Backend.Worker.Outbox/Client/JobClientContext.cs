using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Utilities.RequestDetail;

namespace Sharecode.Backend.Worker.Outbox.Client;

public class JobClientContext : IHttpClientContext
{
    public bool IsApiRequest { get; }
    public string? EmailAddress { get; }
    public Task<Guid?> GetUserIdentifierAsync()
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetNonTrackingUserAsync()
    {
        throw new NotImplementedException();
    }

    public string CacheKey { get; set; }
    public bool HasCacheKey { get; }
    public string[] CacheKeyBlock { get; set; }
    public Dictionary<string, HashSet<string>> CacheInvalidRecords { get; }
    public Task<bool> HasPermissionAsync(Permission key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasPermissionAnyAsync(CancellationToken token = default, params Permission[] key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasPermissionAllAsync(CancellationToken token = default, params Permission[] key)
    {
        throw new NotImplementedException();
    }

    public void AddCacheKeyToInvalidate(string module, params string[] keys)
    {
        throw new NotImplementedException();
    }

    public bool TryGetHeader(string key, out string headerValue)
    {
        throw new NotImplementedException();
    }

    public IRequestDetail RequestDetail { get; }
    public bool HasAuthorizationBearer { get; }
}