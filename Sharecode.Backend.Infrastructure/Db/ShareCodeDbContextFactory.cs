using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Infrastructure.Db;

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
        Namespace? nameSpace = keyValueClient.GetKeysOfNamespaceAsync().GetAwaiter().GetResult();

        if (nameSpace == null)
            throw new Exception("Failed to fetch data from Key Vault");
        
        NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder()
        {
            Database = nameSpace.Of(KeyVaultConstants.PsDatabase)?.Value ?? string.Empty,
            Host = nameSpace.Of(KeyVaultConstants.PsHost)?.Value ?? string.Empty,
            Username = nameSpace.Of(KeyVaultConstants.PsUserName)?.Value ?? string.Empty,
            Password = nameSpace.Of(KeyVaultConstants.PsPassword)?.Value ?? string.Empty,
            Port = int.Parse(nameSpace.Of(KeyVaultConstants.PsPort)?.Value ?? "5432"),
            ApplicationName = "Sharecode"
        };
        
        var optionsBuilder = new DbContextOptionsBuilder<ShareCodeDbContext>();
        optionsBuilder.UseNpgsql(connectionStringBuilder.ConnectionString);
        return new ShareCodeDbContext(optionsBuilder.Options);
    }
}