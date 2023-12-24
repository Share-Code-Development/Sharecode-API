using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Domain.Repositories;

public interface ISnippetCommentRepository : IBaseRepository<SnippetComment>
{
    Task<SnippetComment?> GetChildCommentWithParent(Guid commentId, bool track = true, bool simplify = true ,CancellationToken token = default);
}