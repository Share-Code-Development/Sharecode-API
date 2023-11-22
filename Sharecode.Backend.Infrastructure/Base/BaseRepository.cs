using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Infrastructure.Exceptions;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{

    protected readonly ShareCodeDbContext DbContext;
    protected DbSet<TEntity> Table => DbContext.Set<TEntity>();

    public BaseRepository(ShareCodeDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public void Add(TEntity entity)
    {
        Table.Add(entity);
    }

    public async Task AddAsync(TEntity entity, CancellationToken token)
    {
        await Table.AddAsync(entity, token);
    }

    public void Delete(Guid id)
    {
        TEntity? entity = Table.Find(id);
        if (entity == null)
        {
            throw new NoEntityFoundException(id);
        }
        
        Delete(entity);
    }

    public void Delete(TEntity entity)
    {
        Table.Remove(entity);
    }
    

    public void Update(TEntity entity)
    {
        Table.Update(entity);
    }

    public async Task<TEntity?> GetAsync(Guid id, bool track = true, CancellationToken token = default, bool includeSoftDeleted = false)
    {
        if (track)
        {
            if (!includeSoftDeleted)
            {
                DbSet<TEntity> entities = Table;
                Console.WriteLine(Table.Count());
                List<TEntity> listAsync = DbContext.Set<TEntity>().ToList();
                return await DbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken: token);
            }
            
            return await Table.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);
        }
        else
        {
            if(!includeSoftDeleted)
                return await Table
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken: token);
            
            return await Table
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken: token);
        }
    }

    public TEntity? Get(Guid id, bool track = true, bool includeSoftDeleted = false)
    {
        if (track)
        {
            return Table.FirstOrDefault(x => x.Id == id && ((includeSoftDeleted) || !x.IsDeleted));
        }
        else
        {
            return Table
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id && ((includeSoftDeleted) || !x.IsDeleted));
        }
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(int skip = 0, int take = 50, bool track = true, ISpecification<TEntity>? specification = null, CancellationToken token = default, bool includeSoftDeleted = false)
    {
        IQueryable<TEntity> baseEntities = Table.AsQueryable();

        baseEntities = baseEntities.Where(x => (includeSoftDeleted) || !x.IsDeleted);
        
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