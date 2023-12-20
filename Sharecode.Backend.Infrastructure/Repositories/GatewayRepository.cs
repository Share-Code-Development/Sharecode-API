using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sharecode.Backend.Domain.Entity.Gateway;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class GatewayRepository : BaseRepository<GatewayRequest> , IGatewayRepository
{
    public GatewayRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder) : base(dbContext, connectionStringBuilder)
    {
    }
}