using Microsoft.EntityFrameworkCore;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Infrastructure.Repositories;

namespace Sharecode.Backend.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShareCodeDbContext _context;
    private Dictionary<Type, object> _repositories;

    public UnitOfWork(ShareCodeDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public void Commit()
    {
        _context.SaveChanges();
    }
    
    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Rollback()
    {
        
    }

    public TRepo GetRepository<TEntity, TRepo>() 
        where TEntity : BaseEntity 
        where TRepo : IBaseRepository<TEntity>
    {
        if (_repositories.ContainsKey(typeof(TEntity)))
        {
            return (TRepo)_repositories[typeof(TEntity)];
        }

        var repository = (TRepo)Activator.CreateInstance(typeof(TRepo), _context);
        _repositories.Add(typeof(TEntity), repository);
        return repository;
    }


    public void Dispose()
    {
        _context.Dispose();
    }
}