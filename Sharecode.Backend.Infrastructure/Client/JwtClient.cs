using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Infrastructure.Client;

public class JwtClient : IJwtClient
{

    private readonly JwtConfiguration _jwtConfiguration;
    private readonly Namespace _keyVaultNamespace;
    
    public JwtClient(IOptions<JwtConfiguration> jwtConfiguration, Namespace keyVaultNamespace)
    {
        _jwtConfiguration = jwtConfiguration.Value;
        this._keyVaultNamespace = keyVaultNamespace;
    }
    
    public JwtClient(JwtConfiguration jwtConfiguration, Namespace keyVaultNamespace)
    {
        _jwtConfiguration = jwtConfiguration;
        this._keyVaultNamespace = keyVaultNamespace;
    }    
    
    public String? GenerateRefreshToken(Guid userId, string secretKey, out Guid tokenIdentifier)
    {
        tokenIdentifier = Guid.NewGuid();
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor();
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));
        claims.Add(new Claim("TokenIdentifier", tokenIdentifier.ToString()));
        tokenDescriptor.Subject = new ClaimsIdentity(claims);
        tokenDescriptor.IssuedAt = DateTime.UtcNow;
        tokenDescriptor.Issuer = _jwtConfiguration.Issuer;
        tokenDescriptor.Audience = _jwtConfiguration.Audience;
        tokenDescriptor.Expires = DateTime.UtcNow.AddDays(1);
        tokenDescriptor.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );

        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken); 
    }

    public string? GenerateAccessToken(User user, string secretKey, List<Claim>? claims = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor();
        if (claims == null)
        {
            claims = new List<Claim>();
        }
        claims.Add(new Claim(ClaimTypes.Name, user.FullName));
        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        tokenDescriptor.Subject = new ClaimsIdentity(claims);
        tokenDescriptor.IssuedAt = DateTime.UtcNow;
        tokenDescriptor.Issuer = _jwtConfiguration.Issuer;
        tokenDescriptor.Audience = _jwtConfiguration.Audience;
        tokenDescriptor.Expires = DateTime.UtcNow.AddMinutes(60);
        tokenDescriptor.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken);
    }
}