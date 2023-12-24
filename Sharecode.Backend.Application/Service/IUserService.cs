using Sharecode.Backend.Application.Features.Users.GetMySnippets;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Application.Service;

public interface IUserService
{
    Task<bool> IsEmailAddressUnique(string emailAddress, CancellationToken token = default);
    Task<bool> VerifyUserAsync(Guid userId, CancellationToken token = default);
    Task<bool> RequestForgotPassword(string emailAddress, CancellationToken token = default);
    Task<bool> ResetPasswordAsync(Guid userId, string password, CancellationToken token = default);

    Task<IReadOnlyList<User>> GetUsersToTagAsync(string searchQuery, int take, int skip, bool includeDeleted = false,
        bool shouldEnableTagging = true, CancellationToken token = default);

    Task<List<MySnippetsDto>> ListUserSnippets(Guid userId, bool onlyOwned = false, bool recentSnippets = true,
        int skip = 0, int take = 20, string order = "ASC", string orderBy = "ModifiedAt", string searchQuery = null, 
        CancellationToken cancellationToken = default);
}