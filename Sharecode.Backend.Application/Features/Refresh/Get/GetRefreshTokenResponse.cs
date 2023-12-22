namespace Sharecode.Backend.Application.Features.Refresh.Get;

public class GetRefreshTokenResponse
{
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public string AccessToken { get; set; }
}