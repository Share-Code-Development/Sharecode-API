using Microsoft.EntityFrameworkCore;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{

    private readonly ShareCodeDbContext _dbContext;
    public UserRepository(ShareCodeDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }


    public void Register(User user)
    {
        Add(user);
    }

    public async Task<User?> GetUserIncludingAccountSettings(Guid userId, CancellationToken token = default)
    {
        return await Table
            .AsNoTracking()
            .Include(x => x.AccountSetting)
            .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, cancellationToken: token);

    }

    public async Task<bool> IsEmailAddressUnique(string emailAddress, CancellationToken token = default)
    {
        return await (_dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(x => x.EmailAddress == emailAddress && !x.IsDeleted, token)) == false;
    }

    public async Task<User?> GetUserByIdIncludingAccountSettings(Guid userId, bool includeAccountSettings = false,
        CancellationToken token = default)
    {
        if (includeAccountSettings)
        {
            return await Table
                .AsNoTracking()
                .Include(x => x.AccountSetting)
                .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, cancellationToken: token);
        }
        else
        {
            return await Table
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId && !x.IsDeleted, cancellationToken: token);
        }
    }

    public async Task<User?> GetUserByEmailIncludingAccountSettings(string emailAddress, bool includeAccountSettings = false,
        CancellationToken token = default)
    {
        if (includeAccountSettings)
        {
            return await Table
                .AsNoTracking()
                .Include(x => x.AccountSetting)
                .FirstOrDefaultAsync(x => x.EmailAddress == emailAddress && !x.IsDeleted, cancellationToken: token);
        }
        else
        {
            return await Table
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmailAddress == emailAddress && !x.IsDeleted, cancellationToken: token);
        }
    }
}