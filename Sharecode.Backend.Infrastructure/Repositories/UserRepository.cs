using Microsoft.EntityFrameworkCore;
using Npgsql;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Base;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Db.Extensions;

namespace Sharecode.Backend.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{

    private readonly ShareCodeDbContext _dbContext;
    public UserRepository(ShareCodeDbContext dbContext, NpgsqlConnectionStringBuilder connectionStringBuilder) : base(dbContext, connectionStringBuilder)
    {
        _dbContext = dbContext;
    }
    
    public async Task<User?> GetAsync(string emailAddress, bool track = true, CancellationToken token = default,
        bool includeSoftDeleted = false)
    {
        var queryable = Table
            .SetTracking(track);
        queryable.Include(x => x.AccountSetting);
        if (!includeSoftDeleted)
            queryable = queryable.Where(x => x.EmailAddress == emailAddress && !x.IsDeleted);
        
        return await queryable.FirstOrDefaultAsync(cancellationToken: token);
    }

    public void Register(User user)
    {
        Add(user);
    }
    
    public async Task<EmailState> IsEmailAddressUnique(string emailAddress, CancellationToken token = default)
    {
        var possibleUser = await Table
            .SetTracking(false)
            .Where(x => x.EmailAddress == emailAddress)
            .Select(x => new
            {
                EmailAddress = x.EmailAddress,
                IsDeleted = x.IsDeleted        
            })
            .FirstOrDefaultAsync(token);


        if (possibleUser == null)
            return EmailState.NotPresent;

        if (possibleUser.IsDeleted)
            return EmailState.Deleted;

        return EmailState.Present;
    }
    
    public async Task<User?> GetUserDetailsById(Guid userId, bool includeAccountSettings = false,
        bool trackUser = false,
        CancellationToken token = default)
    {
        var query = Table
            .SetTracking(trackUser)
            .Where(x => x.Id == userId && !x.IsDeleted);

        //If include Settings is true that means the user has permission to view all data
        //If not only select the limited data
        if (includeAccountSettings)
        {
            query = query.Include(x => x.AccountSetting);
        }
        else
        {
            query = query.Select(x => new User()
            {
                EmailAddress = x.EmailAddress,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                ProfilePicture = x.ProfilePicture,
                Visibility = x.Visibility,
                Id = x.Id
            });
        }

        return await query.FirstOrDefaultAsync(cancellationToken: token);
    }
    
    public async Task<User?> GetUserDetailsByEmailAddress(
        string emailAddress,
        bool includeAccountSettings = false,
        bool trackUser = false,
        CancellationToken token = default)
    {
        var query = Table
            .SetTracking(trackUser)
            .Where(x => x.EmailAddress == emailAddress && !x.IsDeleted);

        //If include Settings is true that means the user has permission to view all data
        //If not only select the limited data
        if (includeAccountSettings)
        {
            query = query.Include(x => x.AccountSetting);
        }
        else
        {
            query = query.Select(x => new User()
            {
                EmailAddress = x.EmailAddress,
                FirstName = x.FirstName,
                MiddleName = x.MiddleName,
                LastName = x.LastName,
                ProfilePicture = x.ProfilePicture,
                Visibility = x.Visibility,
                Id = x.Id
            });
        }

        return await query.FirstOrDefaultAsync(cancellationToken: token);
    }
    
    public async Task<List<User>> GetUsersForMentionWithNotificationSettings(HashSet<Guid> userIds,
        CancellationToken token = default)
    {
        return await Table.AsNoTracking()
            .Include(x => x.AccountSetting)
            .Where(x => !x.IsDeleted && userIds.Contains(x.Id))
            .Select(x => new User
            {
                Id = x.Id,
                EmailAddress = x.EmailAddress,
                FirstName = x.FirstName,
                LastName = x.LastName,
                MiddleName = x.MiddleName,
                ProfilePicture = x.ProfilePicture,
                Visibility = AccountVisibility.Public,
                AccountSetting = new AccountSetting()
                {
                    EnableNotificationsForMentions = x.AccountSetting.EnableNotificationsForMentions
                }
            }).ToListAsync(token);
    }


    public async Task<List<User>> GetUsersWithEnabledNotification(HashSet<Guid> userIds, CancellationToken token = default)
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
                ProfilePicture = x.ProfilePicture,
                Visibility = AccountVisibility.Public,
                AccountSetting = new AccountSetting()
                {
                    EnableNotificationsForMentions = x.AccountSetting.EnableNotificationsForMentions
                }
            }).ToListAsync(token);
    }
}