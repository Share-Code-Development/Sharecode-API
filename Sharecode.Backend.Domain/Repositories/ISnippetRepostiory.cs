using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Helper;

namespace Sharecode.Backend.Domain.Repositories;

public interface ISnippetRepository : IBaseRepository<Snippet>
{
    Task<SnippetAccessPermission> GetSnippetAccess(Guid snippetId, Guid requestedUser);
}