using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Outbox;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Infrastructure.Base;

public class UnitOfWork(ShareCodeDbContext context, ILogger logger) : IUnitOfWork
{
    public void Commit()
    {
        DateTime unitOfWorkStarted = DateTime.Now;
        SetModified();
        ConvertDomainEvents();
        EnsureTimeZoneForNg();
        try
        {
            DateTime started = DateTime.Now;
            context.SaveChanges();
            DateTime ended = DateTime.Now;
            logger.Information("Completed unit of work execution at {UoWTime} ms. Database persistence took {Persistence} ms", ended - unitOfWorkStarted, ended - started);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.Error(ex, "Concurrency conflict detected when committing changes.");
            HandleConcurrencyException(ex);
            context.SaveChanges();
        }
        
    }
    
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        DateTime unitOfWorkStarted = DateTime.Now;
        SetModified();
        ConvertDomainEvents();
        EnsureTimeZoneForNg();
        try
        {
            DateTime started = DateTime.Now;
            await context.SaveChangesAsync(cancellationToken);
            DateTime ended = DateTime.Now;
            logger.Information("Completed unit of work execution at {UoWTime} ms. Database persistence took {Persistence} ms", ended - unitOfWorkStarted, ended - started);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            logger.Error(ex, "Concurrency conflict detected when committing changes asynchronously.");
            HandleConcurrencyException(ex);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public void Rollback()
    {
        
    }
    
    
    
    public void Dispose()
    {
        context.Dispose();
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
        var events = context.ChangeTracker.Entries<BaseEntity>()
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
                OccuredOnUtc = DateTime.UtcNow,
                Type = x.GetType().Name,
                Content = JsonConvert.SerializeObject(x, new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.All
                })
            })
            .ToList();

        context.Set<OutboxMessage>().AddRange(events);
    }
    
    private void SetModified()
    {
        var entities = context.ChangeTracker.Entries()
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
                        logger.Information("Delete has been called on again already deleted. Id: {EntityId}!", baseEntity.Id);
                        continue;
                    }
                    
                    entity.State = EntityState.Modified;
                    baseEntity.SoftDeleteEntity();
                }
            }
        }
    }
    

    private void EnsureTimeZoneForNg()
    {
        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is DateTime)
                {
                    var currentValue = (DateTime)property.CurrentValue;
                    property.CurrentValue = DateTime.SpecifyKind(currentValue, DateTimeKind.Utc);
                }
            }
        }
    }
}