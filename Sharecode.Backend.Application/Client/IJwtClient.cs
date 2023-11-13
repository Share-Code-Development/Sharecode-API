using System.Security.Claims;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Client;

public interface IJwtClient
{
    String? GenerateRefreshToken(Guid userId, string secretKey, out Guid tokenIdentifier);

    string? GenerateAccessToken(User user, string secretKey, List<Claim>? claims = null);
}