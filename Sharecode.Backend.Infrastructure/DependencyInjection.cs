using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Sharecode.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection RegisterInfrastructureLayer(this IServiceCollection collection)
    {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = "alen-personal.database.windows.net",
            InitialCatalog = "Alen",
            UserID = "alen-admin-private",
            IntegratedSecurity = true,
            Password = "AW7oF0LUI73TC9v6FOTx72idn7bUW"
        };
        collection.AddDbContext<ShareCodeDbContext>(options =>
        {
            options.UseSqlServer(connectionStringBuilder.ConnectionString);
        });

        return collection;
    }
}