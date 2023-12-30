using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Domain.Enums;

/// <summary>
/// Represents the response of a delete operation.
/// </summary>
public struct DeleteResponse
{

    #region Statics

    /// <summary>
    /// Represents a static property that indicates whether an object has been deleted.
    /// </summary>
    /// <value>
    /// A <see cref="DeleteResponse"/> object representing the deleted state.
    /// </value>
    public static DeleteResponse Deleted => new(true);

    /// <summary>
    /// Creates a DeleteResponse object for the specified entity.
    /// </summary>
    /// <param name="type">The type of the entity.</param>
    /// <param name="entityId">The ID of the entity.</param>
    /// <param name="actor">The ID of the actor performing the deletion. Can be null.</param>
    /// <returns>A DeleteResponse object indicating if the deletion was successful or not.</returns>
    public static DeleteResponse DeletedOf(Type type, Guid entityId, Guid? actor) =>
        new(true, null, type.Name, entityId, actor);

    /// <summary>
    /// Creates a DeleteResponse object with the specified arguments.
    /// </summary>
    /// <param name="type">The type of the entity being deleted.</param>
    /// <param name="entityId">The ID of the entity being deleted.</param>
    /// <param name="actor">The ID of the actor performing the deletion.</param>
    /// <returns>A DeleteResponse object indicating the result of the deletion.</returns>
    public static DeleteResponse DeletedOf(string type, Guid entityId, Guid? actor) =>
        new(true, null, type, entityId, actor);

    /// <summary>
    /// Creates a DeleteResponse object for a given BaseEntity that is deleted by an actor.
    /// </summary>
    /// <param name="entity">The BaseEntity object being deleted.</param>
    /// <param name="actor">The unique identifier of the actor who deleted the entity.</param>
    /// <returns>A DeleteResponse object representing the deletion of the entity.</returns>
    public static DeleteResponse DeletedOfEntity(BaseEntity entity, Guid? actor) =>
        new(true, null, entity.GetType().Name, entity.Id, actor);

    /// <summary>
    /// Represents the response when an entity is not found.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="NotFound"/> property to conveniently create a <see cref="DeleteResponse"/> object
    /// with the <c>IsSuccess</c> property set to <c>false</c>,
    /// the <c>ErrorMessage</c> property set to "No entity found",
    /// and the other properties set to <c>null</c>.
    /// </remarks>
    public static DeleteResponse NotFound => new(false, "No entity found", null, null, null);

    /// <summary>
    /// Creates a <see cref="DeleteResponse"/> object indicating that no entity was found with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the entity that was not found.</param>
    /// <returns>A <see cref="DeleteResponse"/> object indicating that no entity was found with the specified ID.</returns>
    public static DeleteResponse NotFoundWithId(Guid id) => new(false, "No entity found", null, id, null);

    /// <summary>
    /// Creates a new DeleteResponse object with the specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>A new DeleteResponse object representing an error.</returns>
    public static DeleteResponse Error(string message) => new(false, message, null, null, null);

    /// <summary>
    /// Creates an error response for a delete operation.
    /// </summary>
    /// <param name="id">The ID of the item that failed to delete.</param>
    /// <param name="message">The error message describing the reason for the failure.</param>
    /// <returns>A DeleteResponse object indicating a failed delete operation.</returns>
    public static DeleteResponse Error(Guid id, string message) => new(false, message, null, id, null);

    #endregion

    public bool Status { get; }
    public string? Message { get;  }
    public string? Type { get; }
    public Guid? EntityId { get; }
    public Guid? Actor { get;  }

    internal DeleteResponse(bool status)
    {
        Status = status;
    }

    internal DeleteResponse(bool status, string? message, string? type, Guid? entityId, Guid? actor)
    {
        Status = status;
        Message = message ?? string.Empty;
        Type = type ?? string.Empty;
        EntityId = entityId ?? Guid.Empty;
        Actor = actor ?? Guid.Empty;
    }
}