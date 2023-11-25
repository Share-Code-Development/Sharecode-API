using System.Security.Claims;
using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Application.Features.Refresh.Get;

public class GetRefreshTokenCommandHandler
    (
        IHttpClientContext context,
        IJwtClient jwtClient,
        Namespace keyValueNamespace,
        IRefreshTokenService refreshTokenService
    )
    : IRequestHandler<GetRefreshTokenCommand, GetRefreshTokenResponse>
{
    public async Task<GetRefreshTokenResponse> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenSecretKey = keyValueNamespace.Of(KeyVaultConstants.JwtRefreshTokenSecretKey)?.Value ?? string.Empty;
        var refreshTokenSecretEncryption = keyValueNamespace.Of(KeyVaultConstants.JwtRefreshTokenEncryptionKey)?.Value ?? string.Empty;
        
        if (string.IsNullOrEmpty(refreshTokenSecretKey) ||
            string.IsNullOrEmpty(refreshTokenSecretKey))
            throw new UnauthorizedAccessException($"Failed to validate the authority of the user from server!");
        
        var claims = jwtClient.ValidateToken(request.RefreshToken, refreshTokenSecretKey, refreshTokenSecretEncryption);
        var enumerable = claims.ToList();
        if (!enumerable.Any())
        {
            throw new UnauthorizedAccessException("Failed to validate the authority of the user");
        }

        var tokenIdentifier = enumerable.FirstOrDefault(x => x.Type == "TokenIdentifier");
        var tokenIssuedFor = enumerable.FirstOrDefault(x => x.Type == "nameid");

        if (tokenIdentifier == null || tokenIssuedFor == null)
            throw new UnauthorizedAccessException("Failed to validate the token");

        var rawIdentifier = tokenIdentifier.Value;
        var issuedForRaw = tokenIssuedFor.Value;

        var validIdentifier = Guid.TryParse(rawIdentifier, out var identifier);
        var validIssuedFor = Guid.TryParse(issuedForRaw, out var issuedFor);

        if (!validIssuedFor || !validIdentifier)
            throw new UnauthorizedAccessException("Invalid token data provided!");

        var newToken = await refreshTokenService.ValidateTokenIfPresent(identifier, issuedFor);
        if (!newToken.HasValue)
        {
            throw new UnauthorizedAccessException("Failed to validate the identity of the token!");
        }

        string? refreshToken = jwtClient.GenerateRefreshToken(issuedFor, refreshTokenSecretKey, refreshTokenSecretEncryption, ref newToken);
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedAccessException("Failed to create a new identity with the token!");
        }
        
        return new GetRefreshTokenResponse()
        {
            RefreshToken = refreshToken,
            Expiry = DateTime.UtcNow.AddDays(2)
        };
    }
}