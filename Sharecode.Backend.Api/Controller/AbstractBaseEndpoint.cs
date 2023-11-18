using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Client;

namespace Sharecode.Backend.Api.Controller;

public abstract class AbstractBaseEndpoint(IDistributedCache cache, IHttpClientContext requestContext,
        ILogger<AbstractBaseEndpoint> logger)
    : ControllerBase
{
    private readonly IDistributedCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ILogger<AbstractBaseEndpoint> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    protected IHttpClientContext AppRequestContext { get; } = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(1.5);
    
    private const string CacheControlHeader = "Cache-Control";
    private const string NoCache = "no-cache";
    private const string NoStore = "no-store";
    private const string CacheStatusHeader = "X-Cache-Status";
    private const string SkippedDueToDirective = "Skipped due to directive";

    protected async Task<TEntity?> ScanAsync<TEntity>()
    {
        try
        {
            if (ShouldSkipCache(NoCache))
            {
                Response.Headers.Add(CacheStatusHeader, SkippedDueToDirective);
                return default;
            }

            string? cacheObject = await _cache.GetStringAsync(requestContext.CacheKey);
            if (string.IsNullOrEmpty(cacheObject))
                return default;

            return JsonConvert.DeserializeObject<TEntity>(cacheObject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache retrieval");
            return default;
        }
    }

    protected async Task StoreCache(object entityResponse, TimeSpan? ttl = null)
    {
        try
        {
            if (ShouldSkipCache(NoStore))
            {
                Response.Headers.Add(CacheStatusHeader, SkippedDueToDirective);
                return;
            }

            string cacheObject = JsonConvert.SerializeObject(entityResponse);
            if (string.IsNullOrEmpty(cacheObject))
                return;

            ttl ??= CacheTtl;
            var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(ttl.Value);
            await _cache.SetStringAsync(requestContext.CacheKey, cacheObject, cacheEntryOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache store");
        }
    }

    private bool ShouldSkipCache(string directive)
    {
        return TryGetHeader(CacheControlHeader, out var headerValue) && headerValue.Equals(directive, StringComparison.OrdinalIgnoreCase);
    }

    protected bool TryGetHeader(string header, [MaybeNullWhen(false)] out string response)
    {
        if (Request.Headers.TryGetValue(header, out var data))
        {
            response = data;
            return true;
        }

        response = null;
        return false;
    }

    protected void FrameCacheKey(string key)
    {
        requestContext.CacheKey = key;
    }
}
