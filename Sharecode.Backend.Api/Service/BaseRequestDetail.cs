using Sharecode.Backend.Utilities.RequestDetail;

namespace Sharecode.Backend.Api.Service;

public class BaseRequestDetail : IRequestDetail
{
    private const string UserAgentHeader = "User-Agent";
    private const string XForwardedForHeader = "X-Forwarded-For";
    private const string CfConnectingIpHeader = "CF-Connecting-IP";
    private const string CfIpCountry = "CF-IPCountry";
    private const string XLocalAddress = "X-Local-Address";
    private const string CfRayHeader = "CF-RAY";
    private const string CfVisitorHeader = "CF-Visitor";

    private readonly IHttpContextAccessor _contextAccessor;
    private string? _connectingAddress { get; set; }
    private string? _originCountry { get; set; }
    private readonly List<string> _xForwardedFor = new();
    private string? _userAgent { get; set; }
    public BaseRequestDetail(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        IsCloudflareEnvironment = HasCloudflareHeaders();
        PopulateDefaults();
    }
    
    
    public bool IsCloudflareEnvironment { get; set; }
    public string? UserAgent
    {
        get => _userAgent;
        set => _userAgent = value;
    }

    public string? ConnectingAddress
    {
        get => _connectingAddress;
        set => _connectingAddress = value;
    }

    public string? OriginCountry
    {
        get => _originCountry;
        set => _originCountry = value;
    }

    public IReadOnlyList<string> XForwardedFor
    {
        get => _xForwardedFor;
        set => throw new NotImplementedException();
    }

    private void PopulateDefaults()
    {
        if (_contextAccessor?.HttpContext?.Request.Headers.TryGetValue(UserAgentHeader, out var userAgent) ?? false)
        {
            _userAgent = userAgent;
        }
        
        if (_contextAccessor?.HttpContext?.Request.Headers.TryGetValue(XForwardedForHeader, out var headerValue) ?? false)
        {
            if (string.IsNullOrEmpty(headerValue))
            {
                var forwardedHeader = headerValue.ToString();
                _xForwardedFor.AddRange(forwardedHeader.Split(", "));
            }
        }
        
        if (_contextAccessor?.HttpContext?.Request.Headers.TryGetValue(CfIpCountry, out var country) ?? false)
        {
            _originCountry = country;
        }
        
        if (_contextAccessor?.HttpContext?.Request.Headers.TryGetValue(IsCloudflareEnvironment ? CfConnectingIpHeader : XLocalAddress, out var value) ?? false)
        {
            _connectingAddress = value;
        }
    }

    private bool HasCloudflareHeaders()
    {
        var hasCfConnectingIpAddress = _contextAccessor?.HttpContext?.Request.Headers.ContainsKey(CfConnectingIpHeader);
        var hasCfIpCountry = _contextAccessor?.HttpContext?.Request.Headers.ContainsKey(CfIpCountry);
        var hasCfRayHeader = _contextAccessor?.HttpContext?.Request.Headers.ContainsKey(CfRayHeader);
        var hasCfVisitorHeader = _contextAccessor?.HttpContext?.Request.Headers.ContainsKey(CfVisitorHeader);

        return hasCfConnectingIpAddress.HasValue && hasCfConnectingIpAddress.Value &&
               hasCfIpCountry.HasValue && hasCfIpCountry.Value &&
               hasCfRayHeader.HasValue && hasCfRayHeader.Value &&
               hasCfVisitorHeader.HasValue && hasCfVisitorHeader.Value;
    }
    
}