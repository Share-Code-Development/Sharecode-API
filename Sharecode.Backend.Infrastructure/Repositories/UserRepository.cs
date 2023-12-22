using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{

    private readonly ShareCodeDbContext _dbContext;
    public UserRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder) : base(dbContext, connectionStringBuilder)
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

    public async Task<User?> GetUserByIdIncludingAccountSettings(Guid userId, bool includeAccountSettings = false , bool trackUser = false,
        CancellationToken token = default)
    {
        var query = Table.Where(x => x.Id == userId && !x.IsDeleted);

        if (includeAccountSettings)
        {
            query = query.Include(x => x.AccountSetting);
        }

        if (!trackUser)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken: token);
    }

    public async Task<User?> GetUserByEmailIncludingAccountSettings(
        string emailAddress, 
        bool includeAccountSettings = false, 
        bool trackUser = false,
        CancellationToken token = default)
    {
        var query = Table.Where(x => x.EmailAddress == emailAddress && !x.IsDeleted);

        if (includeAccountSettings)
        {
            query = query.Include(x => x.AccountSetting);
        }

        if (!trackUser)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(cancellationToken: token);
    }

    public async Task<List<User>> GetNotificationEnabledUserAsync(HashSet<Guid> userIds, CancellationToken token = default)
    {
        return await Table.AsNoTracking()
            .Include(x => x.AccountSetting)
            .Where(x => x.AccountSetting.EnableNotificationsForMentions && !x.IsDeleted && userIds.Contains(x.Id))
            .Select(x => new User
            {
                Id = x.Id,
                EmailAddress = x.EmailAddress,
                FirstName = x.FirstName,
                LastName = x.LastName,
                MiddleName = x.MiddleName,
                Visibility = AccountVisibility.Public
            }).ToListAsync(token);
    }
}