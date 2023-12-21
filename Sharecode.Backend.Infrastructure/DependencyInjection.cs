using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Client;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Repositories;
using Sharecode.Backend.Infrastructure.Service;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.KeyValue;
using Sharecode.Backend.Utilities.RedisCache;
using StackExchange.Redis;

namespace Sharecode.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructureLayer(this IServiceCollection collection, Namespace nameSpace, bool development = false)
    {
        if (nameSpace == null)
            throw new Exception("Failed to fetch data from Key Vault");
        
        
        #region PostgreSQL

        NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder()
        {
            Database = nameSpace.Of(KeyVaultConstants.PsDatabase)?.Value ?? string.Empty,
            Host = nameSpace.Of(KeyVaultConstants.PsHost)?.Value ?? string.Empty,
            Username = nameSpace.Of(KeyVaultConstants.PsUserName)?.Value ?? string.Empty,
            Password = nameSpace.Of(KeyVaultConstants.PsPassword)?.Value ?? string.Empty,
            Port = int.Parse(nameSpace.Of(KeyVaultConstants.PsPort)?.Value ?? "5432"),
            ApplicationName = development ? "Sharecode-dev" : "Sharecode"
        };

        collection.AddSingleton<NpgsqlConnectionStringBuilder>(connectionStringBuilder);
        
        if (development)
        {
            connectionStringBuilder.IncludeErrorDetail = true;
        }

        var loggerFactory = LoggerFactory.Create(b => b.AddSerilog());
        NpgsqlLoggingConfiguration.InitializeLogging(loggerFactory);

        collection.AddDbContext<ShareCodeDbContext>(options =>
        {
            options.UseNpgsql(connectionStringBuilder.ConnectionString);
            if (!development) return;
            
            options.LogTo(Console.WriteLine);
            Console.WriteLine($"Adding Relational database log to Console for development.");
        });

        #endregion

        #region Redis

        collection.AddShareCodeRedisClient(nameSpace);

        #endregion

        #region Base

        collection.AddScoped<IShareCodeDbContext, ShareCodeDbContext>();
        collection.AddScoped<IUnitOfWork, UnitOfWork>();
        collection.AddSingleton<ITokenClient, TokenClient>();
        collection.AddSingleton<IJwtClient, JwtClient>();

        #endregion

        #region User
        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        collection.AddScoped<IRefreshTokenService, RefreshTokenService>();
        #endregion

        #region Gateway
        collection.AddScoped<IGatewayService, GatewayService>();
        collection.AddScoped<IGatewayRepository, GatewayRepository>();
        #endregion

        #region Snippet

        collection.AddScoped<ISnippetService, SnippetService>();
        collection.AddScoped<ISnippetRepository, SnippetRepository>();

        #endregion
        
        return collection;
    }
    
    public static IServiceCollection AddShareCodeRedisClient(this IServiceCollection service, Namespace keyValueClient)
    {
        string redisConnectionString = keyValueClient.Of(KeyVaultConstants.RedisConnectionString)?.Value ?? string.Empty;
        string redisUserName = keyValueClient.Of(KeyVaultConstants.RedisConnectionUserName)?.Value ?? string.Empty;
        string redisPassword = keyValueClient.Of(KeyVaultConstants.RedisConnectionPassword)?.Value ?? string.Empty;
        
        string[]? strings = redisConnectionString.Split(":");
        DnsEndPoint endPoint = new DnsEndPoint(strings[0], Convert.ToInt32(strings[1]));
        var configurationOption = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            User = redisUserName,
            Password = redisPassword,
            EndPoints = {endPoint}
        };

        service.AddSingleton<ConfigurationOptions>(configurationOption);
        service.AddSingleton<IAppCacheClient, AppCacheClient>();
        return service;
    }
}