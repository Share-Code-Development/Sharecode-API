using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Dto.Snippet;

namespace Sharecode.Backend.Application.Service;

public interface ISnippetService
{
    /// <summary>
    /// Retrieves aggregated data for a snippet. This includes all the data related to a snippet
    /// including  its likes comment count access control etc
    /// </summary>
    /// <param name="snippetId">The ID of the snippet.</param>
    /// <param name="requestedUser">The ID of the user requesting the data. Set to null for anonymous users.</param>
    /// <param name="updateRecent">Specifies whether to update the recently viewed snippets list for the requested user. The default value is false.</param>
    /// <returns>Returns a Task object representing the asynchronous operation. The result is a SnippetDto instance containing the aggregated data for the snippet. If no data is found or the specified snippet ID does not exist, the result will be null.</returns>
    Task<SnippetDto?> GetAggregatedData(Guid snippetId, Guid? requestedUser, bool updateRecent = false);
}