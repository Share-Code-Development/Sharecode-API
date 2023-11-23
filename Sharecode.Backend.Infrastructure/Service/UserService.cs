using Microsoft.EntityFrameworkCore;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Db;

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

    public async Task<bool> VerifyUserAsync(Guid userId, CancellationToken token = default)
    {
        User? user = await _userRepository.GetAsync(userId, token: token);
        
        if (user == null)
            throw new EntityNotFoundException(typeof(User), userId);

        return user.VerifyUser();
    }
}