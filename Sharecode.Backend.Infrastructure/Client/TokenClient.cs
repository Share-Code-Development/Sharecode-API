using Microsoft.Extensions.Options;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Infrastructure.Exceptions.Jwt;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Infrastructure.Client;

public class TokenClient(Namespace keyVaultNamespace, IOptions<JwtConfiguration> jwtConfiguration)
    : ITokenClient
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;
    private readonly IJwtClient _jwtClient = new JwtClient(jwtConfiguration, keyVaultNamespace);
    

    public AccessCredentials Generate(User user)
    {
        KeyValue? accessTokenKeyValue = keyVaultNamespace.Of(KeyVaultConstants.JwtSecretKey);
        KeyValue? refreshTokenKeyValue = keyVaultNamespace.Of(KeyVaultConstants.JwtRefreshTokenSecretKey);
        if (accessTokenKeyValue == null || refreshTokenKeyValue == null)
            throw new JwtFetchKeySecretException($"Failed to fetch the secret-key or refresh-secret-key. Secret Key: {accessTokenKeyValue == null}, Refresh Key: {refreshTokenKeyValue == null}");
        string? accessToken = _jwtClient.GenerateAccessToken(user, accessTokenKeyValue.Value);
        string? refreshToken = _jwtClient.GenerateRefreshToken(user.Id, refreshTokenKeyValue.Value, out var tokenIdentifier);

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