using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Db.Extensions;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetReactionRepository : BaseRepository<SnippetReactions>, ISnippetReactionRepository
{
    public SnippetReactionRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder, ILogger logger) : base(dbContext, connectionStringBuilder, logger)
    {
    }

    public async Task<List<SnippetReactions>> GetReactionsOfUser(Guid snippetId, Guid userId, CancellationToken token = default)
    {
        return await Table
            .Where(x => x.SnippetId == snippetId && x.UserId == userId)
            .ToListAsync(cancellationToken: token);
    }
}