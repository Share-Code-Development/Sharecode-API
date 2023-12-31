using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Domain.Repositories;

public interface ISnippetReactionRepository : IBaseRepository<SnippetReactions>
{
    /// <summary>
    /// Retrieves the reactions of a user for a specific snippet.
    /// </summary>
    /// <param name="snippetId">The unique identifier of the snippet.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of strings representing the reactions of the user.</returns>
    Task<List<SnippetReactions>> GetReactionsOfUser(Guid snippetId, Guid userId, CancellationToken token = default);
}