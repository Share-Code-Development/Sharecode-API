using Npgsql;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetCommentRepository : BaseRepository<Domain.Entity.Snippet.SnippetComment>, ISnippetCommentRepository
{
    public SnippetCommentRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder) : base(dbContext, connectionStringBuilder)
    {
    }
}