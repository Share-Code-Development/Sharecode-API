using System.Net;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
    public static IServiceCollection RegisterInfrastructureLayer(this IServiceCollection collection, Namespace nameSpace, IConfiguration configuration ,bool development = false)
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

        SqlMapper.AddTypeHandler(typeof(Dictionary<int, object>), new JsonObjectTypeHandler ());
        SqlMapper.AddTypeHandler(typeof(List<string>), new JsonObjectTypeHandler ());
        #endregion

        #region Redis

        collection.AddShareCodeRedisCacheClient(nameSpace);
        

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

        #region Snippet & Snippet Comments

        collection.AddScoped<ISnippetService, SnippetService>();
        collection.AddScoped<ISnippetRepository, SnippetRepository>();

        collection.AddScoped<ISnippetCommentService, SnippetCommentService>();
        collection.AddScoped<ISnippetCommentRepository, SnippetCommentRepository>();
        collection.AddScoped<ISnippetLineCommentRepository, SnippetLineCommentRepository>();
        #endregion
        
        #region GroupStateManager
            var clientStateManagerType = configuration.GetValue<string>("LiveGroupStateManagementConfiguration:Implementation");
            if (string.IsNullOrEmpty(clientStateManagerType))
                throw new ApplicationException("LiveGroupStateManagementConfiguration:Implementation has not been defined");

            if (string.Equals(clientStateManagerType, "Redis", StringComparison.OrdinalIgnoreCase))
            {
                collection.AddSharecodeRedisStateManagerClient(nameSpace);
            }
            else if (string.Equals(clientStateManagerType, "PgSQL", StringComparison.OrdinalIgnoreCase))
            {
                
            }
            else
            {
                throw new ApplicationException("LiveGroupStateManagementConfiguration:Implementation must be either Redis or PgSQL");
            }
        #endregion
        return collection;
    }

    private static IServiceCollection AddShareCodeRedisCacheClient(this IServiceCollection service, Namespace keyValueClient)
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

        service.AddKeyedSingleton(DiKeyedServiceConstants.RedisForCache, configurationOption);
        service.AddSingleton<IAppCacheClient, AppCacheClient>();
        return service;
    }
    
    public static IServiceCollection AddSharecodeRedisStateManagerClient(this IServiceCollection service, Namespace keyValueClient)
    {
        if (service.Any(x => x.ServiceType == typeof(IGroupStateManager)))
            throw new ApplicationException("A Group State Manager is already been defined!");
        
        string redisConnectionString = keyValueClient.Of(KeyVaultConstants.SignalRRedisConnectionString)?.Value ?? string.Empty;
        string redisUserName = keyValueClient.Of(KeyVaultConstants.SignalRRedisUserName)?.Value ?? string.Empty;
        string redisPassword = keyValueClient.Of(KeyVaultConstants.SignalRRedisConnectionPassword)?.Value ?? string.Empty;
        
        string[]? strings = redisConnectionString.Split(":");
        DnsEndPoint endPoint = new DnsEndPoint(strings[0], Convert.ToInt32(strings[1]));
        var configurationOption = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            User = redisUserName,
            Password = redisPassword,
            EndPoints = {endPoint}
        };

        service.AddKeyedSingleton(DiKeyedServiceConstants.RedisForSignalRStateManagement, configurationOption);
        service.AddSingleton<IGroupStateManager, RedisGroupStateManager>();
        return service;
    }
}