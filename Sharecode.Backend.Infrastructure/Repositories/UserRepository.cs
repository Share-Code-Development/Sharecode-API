using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ShareCodeDbContext dbContext) : base(dbContext)
    {
    }


    public void Register(User user)
    {
        Add(user);
    }
}