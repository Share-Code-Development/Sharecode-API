using Microsoft.Extensions.DependencyInjection;

namespace Sharecode.Backend.Application.Client;

public interface IGroupStateManager
{

    void OnAppInit(IServiceScope executionScope);
    
    void OnAppDestruct(IServiceScope executionScope);
    
    /// <summary>
    /// Adds a user to a specified group asynchronously.
    /// </summary>
    /// <param name="groupName">The name of the group to add the user to.</param>
    /// <param name="connectionId">The connection ID of the user.</param>
    /// <param name="userIdentifier">The identifier of the user.</param>
    /// <param name="token">Optional cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation. The task result indicates whether the user was successfully added to the group.</returns>
    Task<bool> AddAsync(string groupName, string connectionId, string userIdentifier, CancellationToken token = default);

    /// <summary>
    /// Removes a user from a group asynchronously.
    /// </summary>
    /// <param name="groupName">The name of the group from which to remove the user.</param>
    /// <param name="connectionId">The connection ID of the user to be removed.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a boolean value indicating whether the user was successfully removed from the group.
    /// </returns>
    Task<bool> RemoveAsync(string groupName, string connectionId, CancellationToken token = default);

    /// <summary>
    /// Checks if a connection with the specified connectionId is a member of the group with the specified groupName asynchronously.
    /// </summary>
    /// <param name="groupName">The name of the group to check.</param>
    /// <param name="connectionId">The connection id to check.</param>
    /// <param name="token">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the connection is a member of the group (true) or not (false).</returns>
    Task<bool> IsMemberAsync(string groupName, string connectionId, CancellationToken token = default);

    /// <summary>
    /// Retrieves the members of a group.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <param name="token">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a dictionary containing the members of the group, where the key is the member's username and the value is the member's full name.</returns>
    Task<Dictionary<string, string>> Members(string groupNam, CancellationToken token = default);

    /// <summary>
    /// Retrieves all groups asynchronously.
    /// </summary>
    /// <param name="connectionId">The connection ID.</param>
    /// <param name="token">The cancellation token (optional).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a string representing all the groups.</returns>
    Task<HashSet<string>> GetAllGroupsAsync(string connectionId, CancellationToken token = default);
}