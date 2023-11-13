using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Infrastructure;

public class ShareCodeDbContextFactory : IDesignTimeDbContextFactory<ShareCodeDbContext>
{
    public ShareCodeDbContext CreateDbContext(string[] args)
    {
        string basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Sharecode.Backend.Api");
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .AddEnvironmentVariables()
            .Build();

        KeyValueConfiguration keyValueConfiguration = new KeyValueConfiguration();
        configuration.GetSection("CloudFlareKV").Bind(keyValueConfiguration);

        KeyValueClient keyValueClient = new KeyValueClient(keyValueConfiguration);
        Namespace? result = keyValueClient.GetKeysOfNamespaceAsync().GetAwaiter().GetResult();

        if (result == null)
            throw new Exception("Failed to fetch data from Key Vault");
        
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = result.Of("sql-server-data-source")?.Value ?? string.Empty,
            InitialCatalog = result.Of("sql-server-initial-catalog")?.Value ?? string.Empty,
            UserID = result.Of("sql-server-user-id")?.Value ?? string.Empty,
            IntegratedSecurity = false,
            Password = result.Of("sql-server-password")?.Value ?? string.Empty,
            MultipleActiveResultSets = true
        };
        var optionsBuilder = new DbContextOptionsBuilder<ShareCodeDbContext>();
        optionsBuilder.UseSqlServer(connectionStringBuilder.ConnectionString);
        return new ShareCodeDbContext(optionsBuilder.Options);
    }
}