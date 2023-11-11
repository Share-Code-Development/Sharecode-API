using System.Security.Claims;
using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Service;

public interface IJwtService
{
    String? GenerateRefreshToken(Guid userId, string secretKey, out Guid tokenIdentifier);

    string? GenerateAccessToken(User user, string secretKey, List<Claim>? claims = null);
}