using Sharecode.Backend.Application.Features.Http.Users.GetMySnippets;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Application.Service;

public interface IUserService
{
    /// <summary>
    /// Checks whether the given email address is unique.
    /// </summary>
    /// <param name="emailAddress">The email address to check.</param>
    /// <param name="token">Optional cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains true if the email address is unique; otherwise, false.</returns>
    Task<bool> IsEmailAddressUniqueAsync(string emailAddress, CancellationToken token = default);

    /// <summary>
    /// Verifies if a user with the specified ID exists asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to verify.</param>
    /// <param name="token">A cancellation token that can be used to cancel the verification operation.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is true if the user exists,
    /// and false otherwise.
    /// </returns>
    Task<bool> VerifyUserAsync(Guid userId, CancellationToken token = default);

    /// <summary>
    /// Sends a request to reset the password for the specified email address.
    /// </summary>
    /// <param name="emailAddress">The email address for which the password needs to be reset.</param>
    /// <param name="token">A cancellation token that can be used to cancel the request.</param>
    /// <returns>A task representing the asynchronous operation. The task result will be true if the request was successfully sent; otherwise, false.</returns>
    Task<bool> RequestForgotPasswordAsync(string emailAddress, CancellationToken token = default);

    /// <summary>
    /// Resets the password for a user asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="password">The new password for the user.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation. The result is <c>true</c> if the password is reset successfully; otherwise, <c>false</c>.</returns>
    Task<bool> ResetPasswordAsync(Guid userId, string password, CancellationToken token = default);

    /// <summary>
    /// Retrieves a list of users that can be tagged based on the given search query and other optional parameters.
    /// </summary>
    /// <param name="searchQuery">The search query used to filter the users.</param>
    /// <param name="take">The maximum number of users to retrieve.</param>
    /// <param name="skip">The number of users to skip.</param>
    /// <param name="includeDeleted">Flag indicating whether to include deleted users in the results. Default is false.</param>
    /// <param name="shouldEnableTagging">Flag indicating whether tagging should be enabled for the retrieved users. Default is true.</param>
    /// <param name="token">Cancellation token to cancel the operation if needed.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of users that match the search query and specified parameters.</returns>
    Task<IReadOnlyList<User>> GetUsersToTagAsync(string searchQuery, int take, int skip, bool includeDeleted = false,
        bool shouldEnableTagging = true, CancellationToken token = default);

    /// <summary>
    /// Retrieves a list of snippets for a specified user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="onlyOwned">Optional. Specifies whether to only retrieve snippets owned by the user. Default is false.</param>
    /// <param name="recentSnippets">Optional. Specifies whether to retrieve only recent snippets. Default is true.</param>
    /// <param name="skip">Optional. The number of snippets to skip. Default is 0.</param>
    /// <param name="take">Optional. The number of snippets to retrieve. Default is 20.</param>
    /// <param name="order">Optional. The order in which to retrieve the snippets. Default is "ASC".</param>
    /// <param name="orderBy">Optional. The property to order the snippets by. Default is "ModifiedAt".</param>
    /// <param name="searchQuery">Optional. The search query to filter the snippets by. Default is null.</param>
    /// <param name="cancellationToken">Optional. The cancellation token.</param>
    /// <returns>A list of MySnippetsDto objects representing the snippets.</returns>
    Task<List<MySnippetsDto>> ListUserSnippetsAsync(Guid userId, bool onlyOwned = false, bool recentSnippets = true,
        int skip = 0, int take = 20, string order = "ASC", string orderBy = "ModifiedAt", string searchQuery = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user based on the provided user ID.
    /// </summary>
    /// <param name="userId">The ID of the user to be deleted.</param>
    /// <param name="requestedBy">The ID of the user who requested the deletion.</param>
    /// <param name="softDelete">Specifies whether the user should be soft deleted (default is true).</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation (default is CancellationToken.None).</param>
    /// <returns>A <see cref="Task{bool}"/> representing the asynchronous operation. The task result contains true if the user was successfully deleted; otherwise, false.</returns>
    Task<bool> DeleteUserAsync(Guid userId, Guid requestedBy, bool softDelete = true, CancellationToken token = default);

    /// <summary>
    /// Updates the external metadata for a user asynchronously.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="metadataValues">A dictionary containing the metadata values to be updated.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the update was successful or not.</returns>
    Task<Dictionary<string, object?>> UpdateExternalMetadataAsync(Guid userId, Dictionary<string, object> metadataValues,
        CancellationToken token = default);

    /// <summary>
    /// Deletes external metadata for a user based on the specified keys.
    /// </summary>
    /// <param name="userId">The ID of the user whose external metadata needs to be deleted.</param>
    /// <param name="keys">The set of keys representing the metadata values to delete.</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation (optional).</param>
    /// <returns>A Task representing the asynchronous operation. The task result is a dictionary of deleted metadata keys and their corresponding values.</returns>
    Task DeleteExternalMetadataAsync(Guid userId, HashSet<string> keys,
        CancellationToken token = default);
}