using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class RefreshTokenRepository : BaseRepository<UserRefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ShareCodeDbContext dbContext) : base(dbContext)
    {
        
    }
}