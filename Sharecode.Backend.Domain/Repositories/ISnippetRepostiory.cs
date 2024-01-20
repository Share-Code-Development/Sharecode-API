using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Helper;

namespace Sharecode.Backend.Domain.Repositories;

public interface ISnippetRepository : IBaseRepository<Snippet>
{
    /// <summary>
    /// Get the total storage a particular user is taking
    /// </summary>
    /// <param name="userId">Id of the user to be calculated</param>
    /// <param name="token">The cancellation token</param>
    /// <returns></returns>
    Task<long> GetSnippetSizeOfUsersAsync(Guid userId, CancellationToken token = default);

    /// <summary>
    /// Get the total storage a particular user is taking
    /// </summary>
    /// <param name="specification">The specification to be matched</param>
    /// <param name="token">The cancellation token</param>
    /// <returns></returns>
    Task<long> GetTotalSizeOfSnippetOnSpecification(ISpecification<Snippet> specification,
        CancellationToken token = default);
}