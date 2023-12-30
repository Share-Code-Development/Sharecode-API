using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Domain.Repositories;

public interface ISnippetCommentReactionRepository : IBaseRepository<SnippetCommentReactions>
{
    Task<long> DeleteCommentsOfSnippetAsync(Guid snippetId, CancellationToken token = default);
}