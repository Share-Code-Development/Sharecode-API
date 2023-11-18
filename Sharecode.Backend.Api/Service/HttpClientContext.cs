using System.Security.Claims;
using Sharecode.Backend.Api.Exceptions;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Api.Service;

public class HttpClientContext : IHttpClientContext
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IUserRepository _userRepository;
    private Guid? _userIdentifier = null;
    private User? _user = null;
    private bool? _isApiRequest = null;
    public string? _cacheKey = null;
    
    public HttpClientContext(IHttpContextAccessor contextAccessor, IUserRepository userRepository)
    {
        _contextAccessor = contextAccessor;
        _userRepository = userRepository;
        _isApiRequest = IsApiRequest;
    }

    public bool IsApiRequest
    {
        get
        {
            if (_isApiRequest.HasValue)
                return _isApiRequest.Value;

            _isApiRequest = _contextAccessor.HttpContext?.Request.Headers.ContainsKey("XSC-Api-Key");
            return _isApiRequest!.Value;
        }
    }

    public async Task<Guid?> GetUserIdentifierAsync()
    {
        if (_userIdentifier != null)
        {
            return _userIdentifier;
        }

        _userIdentifier = IsApiRequest ? await GetUserIdentifierFromApiKeyHeader() : GetUserIdentifierFromClaim();
        return _userIdentifier;
    }

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
        set
        {
            if (_cacheKey == null)
                _cacheKey = value;
            
            var fullRequestUrl = $"{_contextAccessor.HttpContext?.Request.Scheme}://{_contextAccessor.HttpContext?.Request.Host}{_contextAccessor.HttpContext?.Request.Path}{_contextAccessor.HttpContext?.Request.QueryString}";
            throw new InvalidCacheAccessException(fullRequestUrl, true);
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
}