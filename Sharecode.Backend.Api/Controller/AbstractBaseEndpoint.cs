using System.Diagnostics.CodeAnalysis;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Api.Controller;

[Route("v1/api/[controller]")]
[ApiController]
public abstract class AbstractBaseEndpoint(IAppCacheClient cache, IHttpClientContext requestContext,
        ILogger<AbstractBaseEndpoint> logger, IMediator mediator)
    : ControllerBase
{
    private readonly IAppCacheClient _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ILogger<AbstractBaseEndpoint> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    protected IHttpClientContext AppRequestContext { get; } = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(3);
    protected CancellationToken RequestCancellationToken => HttpContext.RequestAborted;

    
    private const string CacheControlHeader = "Cache-Control";
    private const string NoCache = "no-cache";
    private const string NoStore = "no-store";
    private const string CacheStatusHeader = "X-Cache-Status";
    private const string SkippedDueToDirective = "Skipped due to directive";

    protected async Task<TEntity?> ScanAsync<TEntity>(bool updateSlidingExpiry = false, CancellationToken token = default)
    {
        try
        {
            if (!AppRequestContext.HasCacheKey)
                return default;
            
            if (ShouldSkipCache(NoCache))
            {
                Response.Headers.Add(CacheStatusHeader, SkippedDueToDirective);
                return default;
            }
            
            string? cacheObject = await _cache.GetCacheAsync(AppRequestContext.CacheKey, updateSlidingExpiry, token);
            if (string.IsNullOrEmpty(cacheObject))
                return default;
            
            return JsonConvert.DeserializeObject<TEntity>(cacheObject, Sharecode.JsonSerializerSettings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during cache retrieval");
            return default;
        }
    }

    protected async Task StoreCacheAsync(object entityResponse, TimeSpan? ttl = null, CancellationToken token = default)
    {
        try
        {
            if (!AppRequestContext.HasCacheKey)
                return;
            
            if (ShouldSkipCache(NoStore))
            {
                Response.Headers.Add(CacheStatusHeader, SkippedDueToDirective);
                return;
            }

            string cacheObject = JsonConvert.SerializeObject(entityResponse, Sharecode.JsonSerializerSettings);
            if (string.IsNullOrEmpty(cacheObject))
                return;

            ttl ??= CacheTtl;
            await _cache.WriteCacheAsync(requestContext.CacheKey, cacheObject, ttl, token: token);
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

    protected bool TryGetHeader(string header, [MaybeNullWhen(false)] out string? response)
    {
        if (Request.Headers.TryGetValue(header, out var data))
        {
            response = data;
            #pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
            return true;
            #pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
        }

        response = null;
        return false;
    }
    
    protected string FrameCacheKey(string module, params string[] keys)
    {
        var finalArray = new string[keys.Length + 1];
        finalArray[0] = module;
        Array.Copy(keys, 0, finalArray, 1, keys.Length);

        requestContext.CacheKeyBlock = finalArray;
        return requestContext.CacheKey;
    }

    protected string GetQuery()
    {
        return Request.QueryString.ToString();
    }

    protected async Task<string> ClearCacheAsync(bool removeSelf = true, CancellationToken token = default)
    {
        
        var identityBuilder = new StringBuilder();
        try
        {
            if (AppRequestContext.CacheInvalidRecords.Any())
            {
                List<string> matchingKeys = new();
                foreach (var (module, keys) in AppRequestContext.CacheInvalidRecords)
                {
                    foreach (var key in keys)
                    {
                        string eachKey = $"{module}-{key}-*";
                        matchingKeys.Add(eachKey);
                        identityBuilder.Append(eachKey);

                        if (token.IsCancellationRequested)
                            break;
                    }
                    if (token.IsCancellationRequested)
                        break;
                }
                
                await _cache.DeleteMatchingKeysAsync(matchingKeys, token).ConfigureAwait(false);
            }

            if (AppRequestContext.HasCacheKey && removeSelf)
            {
                var primaryModule = AppRequestContext.CacheKeyBlock[0];
                var identityBlock = AppRequestContext.CacheKeyBlock[1];
                string identity = $"{primaryModule}-{identityBlock}-*";
                await _cache.DeleteCacheAsync(identity, token).ConfigureAwait(false);
                identityBuilder.Append(identity);   
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"An unknown exception occured during cache handling", ex);
        }

        return identityBuilder.ToString();
    }

    protected bool HasFiles => Request.Form.Files.Any();
    protected bool HasFilesWithName(string fileName) => Request.Form.Files.Any(x => x.Name == fileName);

    protected Stream? GetFileAsStream(string fileName)
    {
        var formFile = Request.Form.Files.GetFile(fileName);
        return formFile?.OpenReadStream();
    }
}
