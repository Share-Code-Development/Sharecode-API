using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Domain.Repositories;

public interface ISnippetRepository : IBaseRepository<Snippet>
{
    Task<Snippet?> GetSnippetById(Guid snippetId);
}