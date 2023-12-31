using System.Data;
using Npgsql;
using Sharecode.Backend.Worker.DbCleanup.Application;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Worker.DbCleanup.Infrastructure;

/// <summary>
/// Represents a PostgreSQL database context.
/// </summary>
public class PgSqlDbContext(NpgsqlConnectionStringBuilder connectionStringBuilder, ILogger logger) : IPgSqlDbContext
{
    public NpgsqlConnectionStringBuilder ConnectionStringBuilder { get; } = connectionStringBuilder;
    
    public NpgsqlConnection? CreateAndOpenConnection()
    {
        try
        {
            var npgsqlConnection = CreateConnection();
            if (npgsqlConnection == null)
                return null;
            
            npgsqlConnection.Open();
            if (npgsqlConnection.State != ConnectionState.Open)
                return null;
            
            return npgsqlConnection;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to open a connection to the PgSQL due to {Message}", e.Message);
            return null;
        }
    }

    public async Task<NpgsqlConnection?> CreateAndOpenConnectionAsync()
    {
        try
        {
            var npgsqlConnection = CreateConnection();
            if (npgsqlConnection == null)
                return null;
            
            await npgsqlConnection.OpenAsync();
            if (npgsqlConnection.State != ConnectionState.Open)
                return null;
            
            return npgsqlConnection;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to open a connection to the PgSQL due to {Message}", e.Message);
            return null;
        }
    }

    public ILogger Logger { get; } = logger;
    public NpgsqlConnection? CreateConnection()
    {
        var connectionString = ConnectionStringBuilder.ToString();
        if (string.IsNullOrEmpty(connectionString))
            return null;

        try
        {
            return new NpgsqlConnection(connectionString);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to create a connection to the PgSQL due to {Message}", e.Message);
            return null;
        }
    }

    
}