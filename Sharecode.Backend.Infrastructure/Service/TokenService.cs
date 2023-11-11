using Microsoft.Extensions.Options;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Exceptions.Jwt;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Infrastructure.Service;

public class TokenService(Namespace keyVaultNamespace, IOptions<JwtConfiguration> jwtConfiguration)
    : ITokenService
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;
    private readonly IJwtService _jwtService = new JwtService(jwtConfiguration, keyVaultNamespace);
    

    public AccessCredentials Generate(User user)
    {
        KeyValue? accessTokenKeyValue = keyVaultNamespace.Of(KeyVaultConstants.JwtSecretKey);
        KeyValue? refreshTokenKeyValue = keyVaultNamespace.Of(KeyVaultConstants.JwtRefreshTokenSecretKey);
        if (accessTokenKeyValue == null || refreshTokenKeyValue == null)
            throw new JwtFetchKeySecretException($"Failed to fetch the secret-key or refresh-secret-key. Secret Key: {accessTokenKeyValue == null}, Refresh Key: {refreshTokenKeyValue == null}");
        string? accessToken = _jwtService.GenerateAccessToken(user, accessTokenKeyValue.Value);
        string? refreshToken = _jwtService.GenerateRefreshToken(user.Id, refreshTokenKeyValue.Value, out var tokenIdentifier);

        if (accessToken == null || refreshToken == null)
            throw new JwtFetchKeySecretException("Failed to generate tokens for user!");

        UserRefreshToken userRefreshToken = new UserRefreshToken()
        {
            TokenIdentifier = tokenIdentifier,
            IssuedFor = user.Id
        };
        
        return new AccessCredentials(accessToken, refreshToken, userRefreshToken);
    }
}