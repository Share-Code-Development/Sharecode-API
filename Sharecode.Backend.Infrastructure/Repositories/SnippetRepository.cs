using Dapper;
using Npgsql;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetRepository : BaseRepository<Snippet>, ISnippetRepository
{
    public SnippetRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder) : base(dbContext, connectionStringBuilder)
    {
    }
}