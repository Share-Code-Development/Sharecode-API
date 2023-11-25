using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Client;

public interface IJwtClient
{
    string? GenerateRefreshToken(Guid userId, string secretKey, string encryptingKey, ref Guid? tokenIdentifier);

    string? GenerateAccessToken(User user, string secretKey, string encryptingKey, List<Claim>? additionalClaims = null);

    IEnumerable<Claim> ValidateToken(string token, string secretKey, string encryptionKey, bool validateExpiry = false);
}