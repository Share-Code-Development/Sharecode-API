using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Db.Extensions;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetCommentRepository : BaseRepository<Domain.Entity.Snippet.SnippetComment>, ISnippetCommentRepository
{
    public SnippetCommentRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder, ILogger logger) : base(dbContext, connectionStringBuilder, logger)
    {
    }

    public new async Task<SnippetComment?> GetAsync(Guid id, bool track = true, CancellationToken token = default, bool includeSoftDeleted = false)
    {
        if (track)
        {
            if (!includeSoftDeleted)
            {
                return await Table
                    .Include(x => x.User)
                    .Include(x => x.Snippet)
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken: token);
            }
            
            return await Table
                .Include(x => x.User)
                .Include(x => x.Snippet)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);
        }
        else
        {
            if(!includeSoftDeleted)
                return await Table
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Snippet)
                    .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken: token);
            
            return await Table
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Snippet)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken: token);
        }
    }

    public async Task<SnippetComment?> GetChildCommentWithParent(Guid commentId, bool track = true, bool simplify = true,CancellationToken token = default)
    {
        if(simplify)
            return await Table
                .SetTracking(track)
                .Where(x => x.Id == commentId)
                .FirstOrDefaultAsync(cancellationToken: token);
        
        return await Table
            .SetTracking(track)
            .Include(x => x.Snippet)
            .ThenInclude(x => x.Owner)
            .Include(x => x.User)
            .Include(x => x.ParentComment)
            .Where(x => x.Id == commentId)
            .FirstOrDefaultAsync(cancellationToken: token);
    }
}