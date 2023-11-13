using Microsoft.EntityFrameworkCore;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Infrastructure.Service;

public class UserService : IUserService
{

    private readonly ShareCodeDbContext _dbContext;
    private readonly IUserRepository _userRepository;

    public UserService(ShareCodeDbContext dbContext, IUserRepository userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task<bool> IsEmailAddressUnique(string emailAddress, CancellationToken token = default)
    {
        return await (_dbContext.Set<User>()
            .AsNoTracking()
            .AnyAsync(x => x.EmailAddress == emailAddress && !x.IsDeleted, token)) == false;
    }
}