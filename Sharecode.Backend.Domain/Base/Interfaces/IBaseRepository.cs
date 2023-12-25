using System.Data;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Domain.Base.Interfaces;

public interface IBaseRepository<TEntity>  where TEntity : BaseEntity
{
    /// <summary>
    /// Adds a new instance of TEntity to the database.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    void Add(TEntity entity);

    /// <summary>
    /// Adds a new entity asynchronously to the database.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    /// <param name="token">The cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAsync(TEntity entity, CancellationToken token);

    /// <summary>
    /// Deletes an entity with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    void Delete(Guid id);

    /// <summary>
    /// Deletes the specified entity from the data source.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    void Delete(TEntity entity);

    /// <summary>
    /// Method to update an entity in the database.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Retrieves an entity asynchronously based on the provided identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="track">Indicates whether to track changes to the retrieved entity. Default is true.</param>
    /// <param name="token">Cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <param name="includeSoftDeleted">Indicates whether to include soft deleted entities in the retrieval. Default is false.</param>
    /// <returns>A task representing the asynchronous operation that eventually yields the retrieved entity. If the entity is not found, null is returned.</returns>
    Task<TEntity?> GetAsync(Guid id, bool track = true, CancellationToken token = default,
        bool includeSoftDeleted = false);

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="track">True to track changes made to the entity, false otherwise.</param>
    /// <param name="includeSoftDeleted">True to include soft deleted entities, false otherwise.</param>
    /// <returns>The retrieved entity, or null if not found.</returns>
    TEntity? Get(Guid id, bool track = true, bool includeSoftDeleted = false);

    /// <summary>
    /// Retrieves a list of entities asynchronously based on the provided parameters.
    /// </summary>
    /// <param name="skip">The number of entities to skip from the beginning of the list. Default value is 0.</param>
    /// <param name="take">The maximum number of entities to retrieve. Default value is 50.</param>
    /// <param name="track">Indicates whether the entities should be tracked by the context. Default value is true.</param>
    /// <param name="specification">An optional specification to filter the entities. Default value is null.</param>
    /// <param name="token">A cancellation token to cancel the operation. Default value is default.</param>
    /// <param name="includeSoftDeleted">Indicates whether soft deleted entities should be included in the result. Default value is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the read-only list of entities.</returns>
    Task<IReadOnlyList<TEntity>> ListAsync(int skip = 0, int take = 50, bool track = true,
        ISpecification<TEntity>? specification = null, CancellationToken token = default,
        bool includeSoftDeleted = false);

    /// <summary>
    /// Deletes a batch of entities that match the specified specification.
    /// </summary>
    /// <param name="specification">The specification to filter the entities to delete. If not provided, all entities of type TEntity will be deleted.</param>
    /// <param name="token">A cancellation token to cancel the delete operation.</param>
    /// <returns>The number of entities deleted.</returns>
    Task<long> DeleteBatchAsync(ISpecification<TEntity>? specification = null, CancellationToken token = default);

    /// <summary>
    /// Create a Dapper context for accessing the database.
    /// </summary>
    /// <returns>An instance of IDbConnection that represents the Dapper context.</returns>
    IDbConnection? CreateDapperContext();
}