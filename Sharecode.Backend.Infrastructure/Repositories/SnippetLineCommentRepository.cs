using Npgsql;
using Serilog;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetLineCommentRepository : BaseRepository<SnippetLineComment>, ISnippetLineCommentRepository
{
    public SnippetLineCommentRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder, ILogger logger) : base(dbContext, connectionStringBuilder, logger)
    {
    }
}