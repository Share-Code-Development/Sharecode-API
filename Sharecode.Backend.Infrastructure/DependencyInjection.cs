using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Client;
using Sharecode.Backend.Infrastructure.Repositories;
using Sharecode.Backend.Infrastructure.Service;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructureLayer(this IServiceCollection collection, Namespace nameSpace)
    {
        if (nameSpace == null)
            throw new Exception("Failed to fetch data from Key Vault");
        
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = nameSpace.Of("sql-server-data-source")?.Value ?? string.Empty,
            InitialCatalog = nameSpace.Of("sql-server-initial-catalog")?.Value ?? string.Empty,
            UserID = nameSpace.Of("sql-server-user-id")?.Value ?? string.Empty,
            IntegratedSecurity = false,
            Password = nameSpace.Of("sql-server-password")?.Value ?? string.Empty,
            MultipleActiveResultSets = true
        };
        collection.AddDbContext<ShareCodeDbContext>(options =>
        {
            options.UseSqlServer(connectionStringBuilder.ConnectionString)
                .LogTo(Console.WriteLine, LogLevel.Information);
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