using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Client;

public interface IHttpClientContext
{
    bool IsApiRequest { get; }
    Task<Guid?> GetUserIdentifierAsync();
    Task<User?> GetNonTrackingUserAsync();
}