using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Infrastructure.Configuration;
using Sharecode.Backend.Infrastructure.Outbox;

namespace Sharecode.Backend.Infrastructure;

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
}