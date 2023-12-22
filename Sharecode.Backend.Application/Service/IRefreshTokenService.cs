using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Service;

public interface IRefreshTokenService
{
    Task<(Guid?, string?, string?, string?, string?)?> ValidateTokenIfPresent(Guid tokenIdentifier, Guid issuedFor);
    Task<UserRefreshToken> GenerateRefreshTokenAsync(Guid issuedFor, bool saveWithUnitOfWork = false, Guid? tokenIdentifier = null, DateTime? expiry = null, CancellationToken token = default);
    Task InvalidateAllOfUserAsync(Guid issuedFor, CancellationToken token = default);
}