using Npgsql;

namespace Sharecode.Backend.Worker.DbCleanup.Application;

/// <summary>
/// Represents a PostgreSQL database context.
/// </summary>
public interface IPgSqlDbContext : IDbContext<NpgsqlConnection, NpgsqlConnectionStringBuilder>
{
    
}