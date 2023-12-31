using System.Data.Common;
using Microsoft.Data.SqlClient;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Worker.DbCleanup.Application;

/// <summary>
/// Represents a database context interface.
/// </summary>
/// <typeparam name="TConnectionType">The type of database connection.</typeparam>
/// <typeparam name="TConnectionConfiguration">The type of database connection configuration.</typeparam>
public interface IDbContext<TConnectionType, TConnectionConfiguration>
    where TConnectionType : DbConnection
    where TConnectionConfiguration : DbConnectionStringBuilder {
    /// <summary>
    /// Represents a connection string builder.
    /// </summary>
    /// <typeparam name="TConnectionConfiguration">The type of connection configuration.</typeparam>
    TConnectionConfiguration ConnectionStringBuilder { get; }

    /// <summary>
    /// Creates a connection of type TConnectionType.
    /// </summary>
    /// <typeparam name="TConnectionType">The type of the connection to create.</typeparam>
    /// <returns>The created connection of type TConnectionType, or null if the connection could not be created.</returns>
    TConnectionType? CreateConnection();

    /// <summary>
    /// Creates and opens a connection of specified type.
    /// </summary>
    /// <typeparam name="TConnectionType">The type of connection to create.</typeparam>
    /// <returns>
    /// The created and opened connection of type TConnectionType if successful;
    /// otherwise, null.
    /// </returns>
    TConnectionType? CreateAndOpenConnection();
    
    /// <summary>
    /// Creates and opens a connection of specified type.
    /// </summary>
    /// <typeparam name="TConnectionType">The type of connection to create.</typeparam>
    /// <returns>
    /// The created and opened connection of type TConnectionType if successful;
    /// otherwise, null.
    /// </returns>
    Task<TConnectionType?> CreateAndOpenConnectionAsync();

    Task ExecuteAsync(Func<DbCommand, Task<bool>> commandFunc);

    /// <summary>
    /// Represents the logger for logging events and messages.
    /// </summary>
    ILogger Logger { get; }
}