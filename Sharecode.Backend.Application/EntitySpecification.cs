using System.Linq.Expressions;
using Sharecode.Backend.Domain.Base;

namespace Sharecode.Backend.Application.Data;

public class EntitySpecification<TEntity> : ISpecification<TEntity> where TEntity : BaseEntity
{
    public Expression<Func<TEntity, bool>> Criteria { get; private set; }
    public List<Expression<Func<TEntity, object>>> Includes { get; private set; } = new List<Expression<Func<TEntity, object>>>();
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; private set; }
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderByDescending { get; private set; }

    public EntitySpecification(Expression<Func<TEntity, bool>> criteria)
    {
        Criteria = criteria;
    }

    public EntitySpecification<TEntity> OrderByAscending(Expression<Func<TEntity, object>> orderBy)
    {
        OrderBy = entities => entities.OrderBy(orderBy);
        return this;
    }

    public EntitySpecification<TEntity>  OrderByDesc(Expression<Func<TEntity, object>> orderBy)
    {
        OrderByDescending = entities => entities.OrderByDescending(orderBy);
        return this;
    }

    public EntitySpecification<TEntity>  Include(Expression<Func<TEntity, object>> include)
    {
        Includes.Add(include);
        return this;
    }
}