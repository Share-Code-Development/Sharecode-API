using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
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

    public async Task ExecuteAsync(Func<DbCommand, Task<bool>> commandFunc)
    {
        NpgsqlConnection? sqlConnection = null; 
        try
        {
            sqlConnection = await CreateAndOpenConnectionAsync();
            if (sqlConnection == null)
            {
                Logger.Error("Failed to open a connection");
                return;
            }

            await using var command = new NpgsqlCommand();
            command.Connection = sqlConnection;
    
            bool success = await commandFunc(command);
            if (success)
            {
                Logger.Information("Command executed successfully");
            }
            else
            {
                Logger.Error("Command execution failed");
            }
        }
        finally
        {
            if (sqlConnection?.State == ConnectionState.Open)
            {
                await sqlConnection.CloseAsync();
            }
            sqlConnection?.Dispose();
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