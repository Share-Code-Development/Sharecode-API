using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Helper;

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

    /// <summary>
    /// Retrieves the access permission for a given snippet ID.
    /// </summary>
    /// <param name="snippetId">The ID of the snippet.</param>
    /// <param name="requestedUser">The ID of the user requesting access.</param>
    /// <param name="checkAdminAccess">Optional. Specifies whether to check for admin access. Default is true.</param>
    /// <returns>The permission level for the requested user on the specified snippet.</returns>
    Task<SnippetAccessPermission> GetSnippetAccess(Guid snippetId, Guid requestedUser, bool checkAdminAccess = true);

    /// <summary>
    /// Deletes a snippet.
    /// </summary>
    /// <param name="snippedId">The ID of the snippet to delete.</param>
    /// <param name="requestedBy">The ID of the user who requested the deletion.</param>
    /// <returns>A task that represents the asynchronous delete operation. The task result contains a boolean value indicating whether the deletion was successful.</returns>
    Task<bool> DeleteSnippet(Guid snippedId, Guid requestedBy);
}