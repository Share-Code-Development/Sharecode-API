using System.Collections.Immutable;
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
    
    public string? GenerateRefreshToken(Guid userId, string secretKey, string encryptingKey, ref Guid? tokenIdentifier)
    {
        tokenIdentifier ??= Guid.NewGuid();
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        var encryptingCredentials = new EncryptingCredentials(
            new SymmetricSecurityKey(Convert.FromBase64String(encryptingKey)),
            SecurityAlgorithms.Aes128KW, // Key wrapping algorithm
            SecurityAlgorithms.Aes128CbcHmacSha256); // Encryption algorithm
        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor();
        List<Claim> claims = new List<Claim>();
        claims.Add(new Claim("nameid", userId.ToString()));
        claims.Add(new Claim("TokenIdentifier", tokenIdentifier.ToString()));
        tokenDescriptor.Subject = new ClaimsIdentity(claims);
        tokenDescriptor.IssuedAt = DateTime.UtcNow;
        tokenDescriptor.Issuer = _jwtConfiguration.Issuer;
        tokenDescriptor.Audience = _jwtConfiguration.Audience;
        tokenDescriptor.Expires = DateTime.UtcNow.AddDays(2);
        tokenDescriptor.SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );
        tokenDescriptor.EncryptingCredentials = encryptingCredentials;

        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(securityToken); 
    }

    public string? GenerateAccessToken(User user, string secretKey, string encryptingKey, List<Claim>? additionalClaims  = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);
        
        var encryptingCredentials = new EncryptingCredentials(
            new SymmetricSecurityKey(Convert.FromBase64String(encryptingKey)),
            SecurityAlgorithms.Aes128KW, // Key wrapping algorithm
            SecurityAlgorithms.Aes128CbcHmacSha256); // Encryption algorithm

        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.EmailAddress)
        };

        // Add any additional claims passed to the method
        if (additionalClaims != null)
        {
            claims.AddRange(additionalClaims);
        }

        // Create a ClaimsIdentity object
        var claimsIdentity = new ClaimsIdentity(claims);

        // Create the SecurityTokenDescriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddMinutes(20), // Token expiration time
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtConfiguration.Issuer,
            Audience = _jwtConfiguration.Audience,
            EncryptingCredentials = encryptingCredentials
        };

        // Create the token
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Return the serialized token
        return tokenHandler.WriteToken(token);
    }

    public IEnumerable<Claim> ValidateToken(string token, string secretKey, string encryptionKey, bool validateExpiry = false)
    {
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = validateExpiry,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtConfiguration.Issuer,
            ValidAudience = _jwtConfiguration.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            TokenDecryptionKey = new SymmetricSecurityKey(Convert.FromBase64String(encryptionKey))
        };

        try
        {
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims;
        }
        catch (SecurityTokenException)
        {
            return ImmutableArray<Claim>.Empty;
        }
    }
}