using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetCommentRepository : BaseRepository<Domain.Entity.Snippet.SnippetComment>, ISnippetCommentRepository
{
    public SnippetCommentRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder) : base(dbContext, connectionStringBuilder)
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
}