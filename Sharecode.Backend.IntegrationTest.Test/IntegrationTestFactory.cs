using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Utilities;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace Sharecode.Backend.IntegrationTest.Test;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:14")
        .WithDatabase("test-integration")
        .WithUsername("user-postgresql")
        .WithPassword("password-postgresql")
        .WithExposedPort(5432)
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(x =>
        {
            var dbDescriptor = x.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<ShareCodeDbContext>));
            if (dbDescriptor is not null)
            {
                x.Remove(dbDescriptor);
            }

            x.AddDbContext<ShareCodeDbContext>(options =>
            {
                options.UseNpgsql(_dbContainer.GetConnectionString());
            });

            var configDescriptor = x.SingleOrDefault(serviceDescriptor =>
                serviceDescriptor.ServiceType == typeof(NpgsqlConnectionStringBuilder) &&
                (string)serviceDescriptor.ServiceKey! == DiKeyedServiceConstants.RedisForCache);
            
            NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder()
            {
                Database = "test-integration",
                Host = _dbContainer.Hostname,
                Username = "user-postgresql",
                Password = "password-postgresql",
                Port = 5432,
                ApplicationName = "IntegrationTesting"
            };
            
            x.AddKeyedSingleton(DiKeyedServiceConstants.RedisForCache, connectionStringBuilder);
            
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    
        using var serviceScope = Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ShareCodeDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}