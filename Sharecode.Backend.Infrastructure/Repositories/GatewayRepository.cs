using Microsoft.EntityFrameworkCore;
using Sharecode.Backend.Domain.Entity.Gateway;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class GatewayRepository : BaseRepository<GatewayRequest> , IGatewayRepository
{
    public GatewayRepository(ShareCodeDbContext dbContext) : base(dbContext)
    {
    }
}