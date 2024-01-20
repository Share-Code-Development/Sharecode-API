using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Base;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly ShareCodeDbContext _dbContext;
    private string? _connectionString = null;
    private NpgsqlConnectionStringBuilder _connectionStringBuilder;
    protected DbSet<TEntity> Table => _dbContext.Set<TEntity>();
    private NpgsqlConnection? _dbConnection = null;
    protected ILogger Logger;

    protected BaseRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder, ILogger logger)
    {
        _dbContext = dbContext;
        _connectionStringBuilder = connectionStringBuilder;
        Logger = logger;
    }

    public IDbConnection? CreateDapperContext()
    {
        if (_dbConnection == null)
        {
            _connectionString ??= _connectionStringBuilder.ToString();
            try
            {
                _dbConnection = new NpgsqlConnection(_connectionString);
            }
            catch (Exception e)
            {
                Log.Error(e, "Failed to open up a new connection from the creds for Dapper");
                return null;
            }
        }

        return _dbConnection;
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
            throw new EntityNotFoundException(typeof(TEntity), id);
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
                return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken: token);
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
        var baseEntities = Table.AsQueryable();
        baseEntities = baseEntities.Where(x => (includeSoftDeleted) || !x.IsDeleted);
        if (specification != null)
        {
            baseEntities = ApplySpecification(baseEntities, specification);
        }
        
        return await baseEntities.Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken: token);
    }

    public async Task<DbCommand> CreateProceduralCommandAsync(string commandName)
    {
        DbCommand? command = _dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = commandName;
        command.CommandType = CommandType.StoredProcedure;
        return command;
    }

    public async Task<long> DeleteBatchAsync(ISpecification<TEntity>? specification = null, CancellationToken token = default)
    {
        IQueryable<TEntity> baseEntities = Table.AsQueryable();
        if (specification != null)
        {
            baseEntities = ApplySpecification(baseEntities, specification);
        }

        return await baseEntities
            .ExecuteDeleteAsync(token);
    }

    protected static IQueryable<TEntity> ApplySpecification(IQueryable<TEntity> query, ISpecification<TEntity> specification)
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