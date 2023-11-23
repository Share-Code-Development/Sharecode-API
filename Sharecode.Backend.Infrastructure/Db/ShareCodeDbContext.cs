using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Infrastructure.Db;

public class ShareCodeDbContext : DbContext, IShareCodeDbContext
{
    
    //public DbSet<User> Users { get; private set; } = null!;
    //public DbSet<AccountSetting> AccountSettings { get; private set; } = null!;
    //public DbSet<OutboxMessage> OutboxMessages { get; private set; } = null!;
    //public DbSet<UserRefreshToken> UserRefreshTokens { get; private set; } = null!;
    public ShareCodeDbContext(DbContextOptions options) : base(options)
    {
    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Assembly assemblyWithConfigurations = GetType().Assembly; //get whatever assembly you want
        modelBuilder.ApplyConfigurationsFromAssembly(assemblyWithConfigurations);
        modelBuilder.HasDefaultSchema("sharecode");
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var idProperty = entityType.FindProperty("Id");

                if (idProperty != null && idProperty.ClrType == typeof(Guid))
                {
                    idProperty.ValueGenerated = ValueGenerated.OnAdd;
                }
                
                var versionProperty = entityType.FindProperty("Version");

                // Check if the entity has a 'Version' property of type byte[]
                if (versionProperty != null && versionProperty.ClrType == typeof(byte[]))
                {
                    // Apply the IsRowVersion configuration
                    modelBuilder.Entity(entityType.ClrType)
                        .Property<uint>("Version")
                        .IsRowVersion();
                }
            }
        }
    }
}