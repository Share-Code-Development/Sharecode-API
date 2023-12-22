using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Service;

public interface IUserService
{
    Task<bool> IsEmailAddressUnique(string emailAddress, CancellationToken token = default);
    Task<bool> VerifyUserAsync(Guid userId, CancellationToken token = default);
    Task<bool> RequestForgotPassword(string emailAddress, CancellationToken token = default);
    Task<bool> ResetPasswordAsync(Guid userId, string password, CancellationToken token = default);

    Task<IReadOnlyList<User>> GetUsersToTagAsync(string searchQuery, int take, int skip, bool includeDeleted = false,
        bool shouldEnableTagging = true, CancellationToken token = default);
    Task<List<User>> GetNotificationEnabledUsersAsync(HashSet<Guid> users, CancellationToken token = default);
}