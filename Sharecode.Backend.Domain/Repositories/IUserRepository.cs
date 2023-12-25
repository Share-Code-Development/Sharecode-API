using System;
using System.Threading;
using System.Threading.Tasks;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>
    /// Retrieves a user asynchronously based on the provided email address.
    /// Use this to manipulate the user entity
    /// </summary>
    /// <param name="emailAddress">The email address of the user to retrieve.</param>
    /// <param name="track">Indicates whether to track changes made to the retrieved user entity. (default: true)</param>
    /// <param name="token">A cancellation token that can be used to cancel the asynchronous operation. (default: default(CancellationToken))</param>
    /// <param name="includeSoftDeleted">Indicates whether to include soft deleted users in the retrieval. (default: false)</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved User object if found; otherwise, it returns null.</returns>
    Task<User?> GetAsync(string emailAddress, bool track = true, CancellationToken token = default,
        bool includeSoftDeleted = false);

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="user">The user object containing registration information.</param>
    void Register(User user);

    /// <summary>
    /// Checks if the given email address is unique.
    /// </summary>
    /// <param name="emailAddress">The email address to check.</param>
    /// <param name="token">Optional cancellation token to cancel the operation.</param>
    /// <returns>Returns a task that represents the asynchronous operation.
    /// The task result contains true if the email address is unique; otherwise, false.</returns>
    Task<bool> IsEmailAddressUnique(string emailAddress, CancellationToken token = default);

    /// <summary>
    /// Retrieves the details of a user by their unique identifier.
    /// NOTE: This method should be only used to return data
    /// Bcs if the include account settings is false, the amount of data it retrieves is very small
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="includeAccountSettings">Optional. Indicates whether to include the user's account settings in the result. Default is false.</param>
    /// <param name="trackUser">Optional. Indicates whether to track the user's activity. Default is false.</param>
    /// <param name="token">Optional. A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user details if found; otherwise, null.</returns>
    Task<User?> GetUserDetailsById(Guid userId, bool includeAccountSettings = false, bool trackUser = false,
        CancellationToken token = default);

    /// <summary>
    /// Retrieves user details by email address.
    /// NOTE: This method should be only used to return data
    /// Bcs if the include account settings is false, the amount of data it retrieves is very small/// 
    /// </summary>
    /// <param name="emailAddress">The email address of the user.</param>
    /// <param name="includeAccountSettings">Optional. Determines whether to include account settings in the retrieved user details. Default is false.</param>
    /// <param name="trackUser">Optional. Determines whether to track the user. Default is false.</param>
    /// <param name="token">Optional. The cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user details if found, otherwise null.</returns>
    Task<User?> GetUserDetailsByEmailAddress(string emailAddress, bool includeAccountSettings = false,
        bool trackUser = false, CancellationToken token = default);

    /// <summary>
    /// Retrieves a list of User objects with notification settings for the given user IDs.
    /// </summary>
    /// <param name="userIds">A HashSet of Guid representing the IDs of the users.</param>
    /// <param name="token">A CancellationToken to cancel the operation (optional).</param>
    /// <returns>A Task that represents the asynchronous operation. The Task result contains a List of User objects.</returns>
    Task<List<User>> GetUsersForMentionWithNotificationSettings(HashSet<Guid> userIds,
        CancellationToken token = default);

    /// <summary>
    /// Retrieves a list of users with enabled notifications based on the provided user IDs.
    /// </summary>
    /// <param name="userIds">The set of user IDs for which to retrieve the users with enabled notifications.</param>
    /// <param name="token">A cancellation token that can be used to cancel the retrieval operation (optional).</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains a list of users with enabled notifications.</returns>
    Task<List<User>> GetUsersWithEnabledNotification(HashSet<Guid> userIds, CancellationToken token = default);
}