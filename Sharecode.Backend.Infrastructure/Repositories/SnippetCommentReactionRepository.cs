using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetCommentReactionRepository : BaseRepository<SnippetCommentReactions>, ISnippetCommentReactionRepository
{
    public SnippetCommentReactionRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder, ILogger logger) : base(dbContext, connectionStringBuilder, logger)
    {
    }

    public async Task<long> DeleteCommentsOfSnippetAsync(Guid snippetId, CancellationToken token = default)
    {
        return 0L;
    }
}