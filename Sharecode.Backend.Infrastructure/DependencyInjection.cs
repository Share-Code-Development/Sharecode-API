using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Client;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Repositories;
using Sharecode.Backend.Infrastructure.Service;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructureLayer(this IServiceCollection collection, Namespace nameSpace)
    {
        if (nameSpace == null)
            throw new Exception("Failed to fetch data from Key Vault");
        
        NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder()
        {
            Database = nameSpace.Of(KeyVaultConstants.PsDatabase)?.Value ?? string.Empty,
            Host = nameSpace.Of(KeyVaultConstants.PsHost)?.Value ?? string.Empty,
            Username = nameSpace.Of(KeyVaultConstants.PsUserName)?.Value ?? string.Empty,
            Password = nameSpace.Of(KeyVaultConstants.PsPassword)?.Value ?? string.Empty,
            Port = int.Parse(nameSpace.Of(KeyVaultConstants.PsPort)?.Value ?? "5432"),
            ApplicationName = "Sharecode",
            IncludeErrorDetail = true
        };

        collection.AddDbContext<ShareCodeDbContext>(options =>
        {
            options.UseNpgsql(connectionStringBuilder.ConnectionString);
        });
        
        collection.AddScoped<IUnitOfWork, UnitOfWork>();
        collection.AddSingleton<ITokenClient, TokenClient>();
        collection.AddSingleton<IJwtClient, JwtClient>();

        #region User
        collection.AddScoped<IUserRepository, UserRepository>();
        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        #endregion

        #region Gateway
        collection.AddScoped<IGatewayService, GatewayService>();
        collection.AddScoped<IGatewayRepository, GatewayRepository>();
        #endregion

        
        return collection;
    }
}