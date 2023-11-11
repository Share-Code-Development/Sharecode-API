using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Infrastructure.Outbox;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShareCodeDbContext _context;
    private readonly ILogger<IUnitOfWork> _log;
    public UnitOfWork(ShareCodeDbContext context, ILogger<IUnitOfWork> logger)
    {
        _context = context;
        _log = logger;
    }

    public void Commit()
    {
        SetModified();
        ConvertDomainEvents();
        _context.SaveChanges();
    }
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        SetModified();
        ConvertDomainEvents();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public void Rollback()
    {
        
    }
    
    
    public void Dispose()
    {
        _context.Dispose();
    }

    private void ConvertDomainEvents()
    {
        var events = _context.ChangeTracker.Entries<BaseEntity>()
            .Select(x => x.Entity)
            .SelectMany(x =>
            {
                var domainEvents = x.DomainEvents;
                x.ClearEvents();

                return domainEvents;
            })
            .Select(x => new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccuredOnUtc = DateTime.Now,
                Type = x.GetType().Name,
                Content = JsonConvert.SerializeObject(x, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All
                })
            })
            .ToList();

        _context.Set<OutboxMessage>().AddRange(events);
    }
    
    private void SetModified()
    {
        var entities = _context.ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entity in entities)
        {
            BaseEntity baseEntity = (entity.Entity as BaseEntity)!;
            if (entity.State == EntityState.Added)
            {
                baseEntity.CreatedAt = DateTime.UtcNow;
                baseEntity.ModifiedAt = DateTime.UtcNow;
                baseEntity.RaiseCreatedEvent();
            }else if (entity.State == EntityState.Modified)
            {
                baseEntity.ModifiedAt = DateTime.UtcNow;
            }else if (entity.State == EntityState.Deleted)
            {
                //If the entity is set to hard delete, It will simple delete, without raising an event
                //So in order to hard delete from database, Set the entity as HardDelete, then call Delete method
                if (baseEntity.HardDelete)
                {
                    continue;
                }
                else
                {
                    if (baseEntity.IsDeleted)
                    {
                        _log.LogInformation($"Delete has been called on again already deleted. Id: {baseEntity.Id}!");
                        continue;
                    }
                    
                    entity.State = EntityState.Modified;
                    baseEntity.SoftDeleteEntity();
                }
            }
        }
    }
}