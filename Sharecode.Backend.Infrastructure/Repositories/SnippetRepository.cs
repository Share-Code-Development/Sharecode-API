using Dapper;
using Npgsql;
using Serilog;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Helper;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Exceptions;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetRepository : BaseRepository<Snippet>, ISnippetRepository
{
    public SnippetRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder, ILogger logger) : base(dbContext, connectionStringBuilder, logger)
    {
    }

    public async Task<SnippetAccessPermission> GetSnippetAccess(Guid snippetId, Guid requestedUser)
    {
        try
        {
            using var dapperContext = CreateDapperContext();
            if (dapperContext == null)
                throw new InfrastructureDownException("Failed to get snippet permissions",
                    "Failed to create dapper context for getting snippet permission");

            return null;
        }
        catch (Exception e)
        {
            if (e is InfrastructureDownException)
                throw;
            
            Logger.Error(e, "An unknown error occured while fetching the permission of user {User} on Snippet {Snippet} due to {Message}", requestedUser, snippetId, e.Message);
            return SnippetAccessPermission.Error();
        }
    }
}