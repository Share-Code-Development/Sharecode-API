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

    
}

