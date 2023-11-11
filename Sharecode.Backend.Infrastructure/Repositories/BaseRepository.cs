using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Infrastructure.Exceptions;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{

    private readonly ShareCodeDbContext _dbContext;
    private readonly DbSet<TEntity> _table;

    public BaseRepository(ShareCodeDbContext dbContext)
    {
        _dbContext = dbContext;
        _table = _dbContext.Set<TEntity>();
    }

    public void Add(TEntity entity)
    {
        _table.Add(entity);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _table.AddAsync(entity);
    }

    public void Delete(Guid id)
    {
        TEntity? entity = _table.Find(id);
        if (entity == null)
        {
            throw new NoEntityFoundException(id);
        }
        
        Delete(entity);
    }

    public void Delete(TEntity entity)
    {
        _table.Remove(entity);
    }
    

    public void Update(TEntity entity)
    {
        _table.Update(entity);
    }

    public async Task<TEntity?> GetAsync(Guid id, bool track = true, CancellationToken token = default)
    {
        if (track)
        {
            return await _table.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);
        }
        else
        {
           return await _table
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);
        }
    }

    public TEntity? Get(Guid id, bool track = true)
    {
        if (track)
        {
            return _table.FirstOrDefault(x => x.Id == id);
        }
        else
        {
            return _table
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(int skip = 0, int take = 50, bool track = true, ISpecification<TEntity>? specification = null, CancellationToken token = default)
    {
        IQueryable<TEntity> baseEntities = _table.AsQueryable();

        if (specification != null)
        {
            baseEntities = ApplySpecification(baseEntities, specification);
        }

        return await baseEntities.Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken: token);
    }

    private static IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> specification)
    {
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }
        if (specification.Includes != null)
        {
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        }
        if (specification.OrderBy != null)
        {
            query = specification.OrderBy(query);
        }
        if (specification.OrderByDescending != null)
        {
            query = specification.OrderByDescending(query);
        }

        return query;
    }
}