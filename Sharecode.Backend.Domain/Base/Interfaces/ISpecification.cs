using System.Linq;
using System.Linq.Expressions;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Domain.Base.Interfaces;

public interface ISpecification<TEntity> where TEntity : BaseEntity 
{
    Expression<Func<TEntity, bool>>? Criteria { get; }
    List<Expression<Func<TEntity, object>>>? Includes { get; }
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy { get; }
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderByDescending { get; }

}