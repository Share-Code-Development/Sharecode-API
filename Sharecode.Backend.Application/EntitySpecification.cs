using System.Linq.Expressions;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Application;

public class EntitySpecification<TEntity>(Expression<Func<TEntity, bool>> criteria) : ISpecification<TEntity>
    where TEntity : BaseEntity
{
    public Expression<Func<TEntity, bool>> Criteria { get; private set; } = criteria;
    public List<Expression<Func<TEntity, object>>> Includes { get; private set; } = new List<Expression<Func<TEntity, object>>>();
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; private set; }
    public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderByDescending { get; private set; }
    public Expression<Func<TEntity, object>>? Select { get; private set; }
    
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