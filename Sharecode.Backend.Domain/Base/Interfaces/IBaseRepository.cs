using System.Linq.Expressions;

namespace Sharecode.Backend.Domain.Base;

public interface IBaseRepository<TEntity>  where TEntity : BaseEntity
{
    void Add(TEntity entity);
    Task AddAsync(TEntity entity);
    void Delete(Guid id);
    void Delete(TEntity entity);
    void Update(TEntity entity);
    Task<TEntity?> GetAsync(Guid id, bool track = true, CancellationToken token = default, bool includeSoftDeleted = false);
    TEntity? Get(Guid id, bool track = true, bool includeSoftDeleted = false);
    Task<IReadOnlyList<TEntity>> ListAsync(int skip = 0, int take = 50, bool track = true, ISpecification<TEntity>? specification = null, CancellationToken token = default, bool includeSoftDeleted = false);



}