using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Infrastructure.Outbox;

namespace Sharecode.Backend.Infrastructure.Base;

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
        try
        {
            _context.SaveChanges();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _log.LogError(ex, "Concurrency conflict detected when committing changes.");
            HandleConcurrencyException(ex);
            _context.SaveChanges();
        }
        
    }
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        SetModified();
        ConvertDomainEvents();
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _log.LogError(ex, "Concurrency conflict detected when committing changes asynchronously.");
            HandleConcurrencyException(ex);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public void Rollback()
    {
        
    }
    
    
    public void Dispose()
    {
        _context.Dispose();
    }

    
    private void HandleConcurrencyException(DbUpdateConcurrencyException ex)
    {
        foreach (var entry in ex.Entries)
        {
            var databaseValues = entry.GetDatabaseValues();
            if (databaseValues != null)
            {
                // Overwrite the database values with the values of the entity that attempted the update
                entry.OriginalValues.SetValues(databaseValues);
            }
        }
    }
    
    /*private void HandleConcurrencyException(DbUpdateConcurrencyException ex)
    {
        foreach (var entry in ex.Entries)
        {
            var proposedValues = entry.CurrentValues;
            var databaseValues = entry.GetDatabaseValues();

            if (databaseValues != null)
            {
                // Iterate through each property and decide how to merge the proposed value and the database value
                foreach (var property in proposedValues.Properties)
                {
                    var proposedValue = proposedValues[property];
                    var databaseValue = databaseValues[property];

                    // TODO: Determine how to merge. This might involve user input, business logic, etc.
                    // e.g., proposedValues[property] = ChooseValue(proposedValue, databaseValue);
                }

                // Update the original values to the database values and retry
                entry.OriginalValues.SetValues(databaseValues);
            }
        }
    }*/

    
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