using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Infrastructure.Configuration;

namespace Sharecode.Backend.Infrastructure;

public class ShareCodeDbContext : DbContext
{
    
    public DbSet<User> Users { get; set; }
    
    public ShareCodeDbContext(DbContextOptions options) : base(options)
    {
        base.SavingChanges += OnSavingChanges;
    }

    private void OnSavingChanges(object? sender, SavingChangesEventArgs e)
    {
        _cleanString();
        SetModified(ChangeTracker);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Assembly assemblyWithConfigurations = GetType().Assembly; //get whatever assembly you want
        modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);
        modelBuilder.HasDefaultSchema("ShareCode");
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var idProperty = entityType.FindProperty("Id");

                if (idProperty != null && idProperty.ClrType == typeof(Guid))
                {
                    idProperty.ValueGenerated = ValueGenerated.OnAdd;
                }
            }
        }
    }
    
    private void SetModified(ChangeTracker tracker)
    {
        var entities = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified))
            .Select(e => e.Entity as BaseEntity);

        foreach (var entity in entities)
        {
            entity?.SetUpdated();
        }
    }
    
    private void _cleanString()
    {
        var changedEntities = ChangeTracker.Entries()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        foreach (var item in changedEntities)
        {
            if (item.Entity == null)
                continue;

            var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                var propName = property.Name;
                var val = (string)property.GetValue(item.Entity, null);

                if (val != null && val.Length > 0)
                {
                    var newVal = TransformString(val);
                    if (!string.Equals(newVal, val, StringComparison.Ordinal))
                        property.SetValue(item.Entity, newVal, null);
                }
            }
        }
    }
    
    private string TransformString(string input)
    {
        string[] persian = new string[10] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

        for (int j=0; j<persian.Length; j++)
            input = input.Replace(persian[j], j.ToString());

        return input;
    }
}