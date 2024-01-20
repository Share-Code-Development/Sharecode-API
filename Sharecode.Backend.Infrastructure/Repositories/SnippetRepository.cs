using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Serilog;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Helper;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Db.Extensions;
using Sharecode.Backend.Infrastructure.Exceptions;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class SnippetRepository : BaseRepository<Snippet>, ISnippetRepository
{
    public SnippetRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder, ILogger logger) : base(dbContext, connectionStringBuilder, logger)
    {
    }


    public Task<long> GetSnippetSizeOfUsersAsync(Guid userId, CancellationToken token = default)
    {
        return Table
            .SetTracking(false)
            .Where(x => x.OwnerId == userId)
            .Select(x => x.Size)
            .SumAsync(cancellationToken: token);
    }

    public Task<long> GetTotalSizeOfSnippetOnSpecification(ISpecification<Snippet> specification, CancellationToken token = default)
    {
        var queryable = Table
            .SetTracking(false)
            .AsQueryable();

        var snippetSpecification = ApplySpecification(queryable, specification);
        return snippetSpecification
            .Select(x => x.Size )
            .SumAsync(token);
    }
}

