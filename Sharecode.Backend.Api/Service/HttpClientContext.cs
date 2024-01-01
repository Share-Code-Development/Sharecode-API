using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Newtonsoft.Json;
using Sharecode.Backend.Api.Exceptions;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.RedisCache;
using Sharecode.Backend.Utilities.RequestDetail;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.Service;

public class HttpClientContext : IHttpClientContext
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly IAppCacheClient _appCacheClient;
    private Guid? _userIdentifier = null;
    private User? _user = null;
    private bool? _isApiRequest = null;
    private string? _cacheKey = null;
    private string? _emailAddress = null;
    private string[]? _cacheKeys = null;
    private IRequestDetail? _requestDetail;
    private bool? _hasAuthorizationBearer = null;
    private readonly Dictionary<string, HashSet<string>> _cacheInvalidateRecords = new();
    private HashSet<Permission>? _permissions;
    
    public HttpClientContext(IHttpContextAccessor contextAccessor, IUserRepository userRepository, IAppCacheClient cacheClient, ILogger logger)
    {
        _contextAccessor = contextAccessor;
        _userRepository = userRepository;
        _isApiRequest = IsApiRequest;
        _appCacheClient = cacheClient;
        _logger = logger;
    }

    public string? EmailAddress
    {
        get
        {
            if (IsApiRequest)
                return null;

            if (string.IsNullOrEmpty(_emailAddress))
            {
                string? emailAddress = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(emailAddress))
                    return null;

                _emailAddress = emailAddress;
            }

            return _emailAddress;
        }
    }

    /// <summary>
    /// Is the request coming from an API Endpoint
    /// </summary>
    public bool IsApiRequest
    {
        get
        {
            if (_isApiRequest.HasValue)
                return _isApiRequest.Value;

            _isApiRequest = _contextAccessor.HttpContext?.Request.Headers.ContainsKey("XSC-Api-Key");
            return _isApiRequest ?? false;
        }
    }

    /// <summary>
    /// Get the user who send the request
    /// </summary>
    /// <returns></returns>
    public async Task<Guid?> GetUserIdentifierAsync()
    {
        if (_userIdentifier != null)
        {
            return _userIdentifier;
        }

        _userIdentifier = IsApiRequest ? await GetUserIdentifierFromApiKeyHeader() : GetUserIdentifierFromClaim();
        return _userIdentifier;
    }

    /// <summary>
    /// Get the user as no tracking
    /// </summary>
    /// <returns></returns>
    public async Task<User?> GetNonTrackingUserAsync()
    {
        if (_user != null)
        {
            return _user;
        }

        Guid? userIdentifier = await GetUserIdentifierAsync();
        User? user = await _userRepository.GetAsync(userIdentifier!.Value, track: false);
        _user = user;
        return _user;
    }

    /// <summary>
    /// What would be the request key, this is a scoped class so it will be same for each and every request
    /// </summary>
    /// <exception cref="InvalidCacheAccessException"></exception>
    public string CacheKey
    {
        get
        {
            if (_cacheKey == null)
            {
                var fullRequestUrl = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}{_contextAccessor.HttpContext?.Request.Path}{_contextAccessor.HttpContext?.Request.QueryString}";
                throw new InvalidCacheAccessException(fullRequestUrl, false);
            }

            return _cacheKey;
        }
        set => _cacheKey = value;
    }

    public bool HasCacheKey => !string.IsNullOrEmpty(_cacheKey);

    public string[] CacheKeyBlock
    {
        get
        {
            if (_cacheKeys == null)
            {
                var fullRequestUrl = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}{_contextAccessor.HttpContext?.Request.Path}{_contextAccessor.HttpContext?.Request.QueryString}";
                throw new InvalidCacheAccessException(fullRequestUrl, false);
            }
            
            return _cacheKeys;
        }
        set
        {
            if (value.Length < 2)
            {
                var fullRequestUrl = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}{_contextAccessor.HttpContext?.Request.Path}{_contextAccessor.HttpContext?.Request.QueryString}";
                throw new InvalidCacheAccessException(value, fullRequestUrl);
            }
            
            if (_cacheKeys == null)
                _cacheKeys = value;
            
            _cacheKeys = value;
            string cacheKey = string.Empty;
            foreach (var key in _cacheKeys)
            {
                if (cacheKey == string.Empty)
                    cacheKey = key;
                else
                    cacheKey += "-" + key;
            }
            _cacheKey = cacheKey;
        }
    }

    public Dictionary<string, HashSet<string>> CacheInvalidRecords => _cacheInvalidateRecords;

    public async Task<bool> HasPermissionAsync(Permission key, CancellationToken token = default)
    {
        var nonTrackingUsr = await GetNonTrackingUserAsync();
        if (nonTrackingUsr == null)
            return false;
        
        return nonTrackingUsr.Permissions.Contains(key);
    }

    public async Task<bool> HasPermissionAnyAsync(CancellationToken token = default, params Permission[] key)
    {
        return await HasPermissionAnyAsync(token, key);
    }

    public async Task<bool> HasPermissionAnyAsync(Permission[] key, CancellationToken token = default)
    {
        var permissions = await GetPermissionsAsync();
        foreach (var permission in key)
        {
            var contains = permissions.Contains(permission);
            if (contains)
                return true;
        }

        return false;
    }

    public async Task<bool> HasPermissionAllAsync(CancellationToken token = default, params Permission[] key)
    {
        return await HasPermissionAllAsync(key, token);
    }

    public async Task<bool> HasPermissionAllAsync(Permission[] key, CancellationToken token = default)
    {
        var permissions = await GetPermissionsAsync();
        foreach (var permission in key)
        {
            var contains = permissions.Contains(permission);
            if (!contains)
                return false;
        }

        return true;
    }

    public void AddCacheKeyToInvalidate(string module, params string[] keys)
    {
        if (_cacheInvalidateRecords.TryGetValue(module, out var currentKeys))
        {
            foreach (var key in keys)
            {
                currentKeys.Add(key);
            }
            return;
        }

        HashSet<string> cacheKeys = new HashSet<string>(keys);
        _cacheInvalidateRecords[module] = cacheKeys;
    }

    public bool TryGetHeader(string header, [MaybeNullWhen(false)] out string response)
    {
        response = null;
        if (_contextAccessor.HttpContext is not { Request.Headers: not null }) return false;
        if (!_contextAccessor.HttpContext.Request.Headers.TryGetValue(header, out var data)) return false;
        response = data;
        #pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
        return true;
        #pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
    }

    public IRequestDetail RequestDetail
    {
        get
        {
            _requestDetail ??= new BaseRequestDetail(_contextAccessor);

            return _requestDetail;
        }
    }

    public bool HasAuthorizationBearer
    {
        get
        {
            if (!_hasAuthorizationBearer.HasValue)
            {
                if (_contextAccessor?.HttpContext?.User == null)
                {
                    _hasAuthorizationBearer = false;
                    return false;
                }
            }

            return _hasAuthorizationBearer!.Value;
        }
    }


    private Guid? GetUserIdentifierFromClaim()
    {
        string? identifierRaw = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(identifierRaw))
            return null;

        if (!Guid.TryParse(identifierRaw, out var identifier))
        {
            return null;
        }

        _userIdentifier = identifier;
        return identifier;
    }

    private async Task<Guid?> GetUserIdentifierFromApiKeyHeader()
    {
        string? apiKey = _contextAccessor.HttpContext?.Request.Headers["XSC-Api-Key"];
        // Add logic to validate and fetch user identifier based on API key
        // For example, query a database or cache to get the user associated with the API key.
        // Ensure to handle errors and return null if the API key is invalid.
        // ...

        return null; // Replace with the actual user identifier obtained from the API key.
    }

    private async Task<HashSet<Permission>> GetPermissionsAsync()
    {
        //If the permission is not yet initialized
        if (_permissions == null)
        {
            var userIdentifier = await GetUserIdentifierAsync();
            //If the user doesn't have a token, set the permission as an empty array 
            if (!userIdentifier.HasValue)
            {
                _logger.Information("No valid user id is available to identify the user");
                _permissions = [];
                return _permissions;
            }
            
            //If the user is already fetched, get the permissions from the user object
            if (_user != null)
            {
                _logger.Information("Permission data for {UserId} has been retrieved from the loaded user object.", _user.Id);
                _permissions = _user.Permissions;
                return _permissions;
            }

            var cacheString = await _appCacheClient.GetCacheAsync(
                $"{CacheModules.InternalUserPermission}-{userIdentifier.Value.ToString()}", true);

            bool fetchFromDb = true;
            if (!string.IsNullOrEmpty(cacheString))
            {
                try
                {
                    _permissions = JsonConvert.DeserializeObject<HashSet<Permission>>(cacheString);
                    if (_permissions == null)
                        fetchFromDb = true;
                    else
                        fetchFromDb = false;
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Fetching Permission From Internal cache succeeded but failed to deserialize the object for user {UserId} due to {Message}", userIdentifier.Value.ToString(),e.Message);
                    fetchFromDb = true;
                }
            }

            if (!fetchFromDb)
            {
                _logger.Information("The permission has been found on the internal cache of the user {UserId}. Retried from the caching unit", userIdentifier.Value);
                return _permissions!;
            }
            
            _permissions = await _userRepository.GetUsersPermissionAsync(userIdentifier.Value);
            await _appCacheClient.WriteCacheAsync($"{CacheModules.InternalUserPermission}-{userIdentifier.Value.ToString()}", JsonConvert.SerializeObject(_permissions), _appCacheClient.DefaultCachingLife);
            _logger.Information("The permission has been loaded from database for user {UserId}", userIdentifier.Value);
        }

        return _permissions;
    }
}