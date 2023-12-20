using Microsoft.EntityFrameworkCore;
using Sharecode.Backend.Application;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Utilities.SecurityClient;

namespace Sharecode.Backend.Infrastructure.Service;

public class UserService : IUserService
{

    private readonly ShareCodeDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly ISecurityClient _securityClient;
    private readonly IHttpClientContext _context;

    public UserService(ShareCodeDbContext dbContext, IUserRepository userRepository, ISecurityClient securityClient, IHttpClientContext context)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
        _securityClient = securityClient;
        _context = context;
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
        
        if (!user.Active)
        {
            throw new AccountTemporarilySuspendedException(user.EmailAddress, user.InActiveReason!);
        }

        return user.VerifyUser();
    }

    public async Task<bool> RequestForgotPassword(string emailAddress, CancellationToken token = default)
    {
        var user = await _userRepository.GetUserByEmailIncludingAccountSettings(emailAddress, false, true, token);
        if (user == null)
            throw new EntityNotFoundException(typeof(User), emailAddress);
        
        if (!user.Active)
        {
            throw new AccountTemporarilySuspendedException(user.EmailAddress, user.InActiveReason!);
        }

        var requestPasswordReset = user.RequestPasswordReset(false);
        return requestPasswordReset;
    }

    public async Task<bool> ResetPasswordAsync(Guid userId, string password, CancellationToken token = default)
    {
        var user = await _userRepository.GetUserByIdIncludingAccountSettings(userId, false, true,  token);
        if (user == null)
            throw new EntityNotFoundException(typeof(User), userId);
        
        if (!user.Active) 
            throw new AccountTemporarilySuspendedException(user.EmailAddress, user.InActiveReason!);
        
        _securityClient.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
        user.UpdatePassword(passwordHash, passwordSalt, _context.RequestDetail);
        return true;
    }

    public Task<List<User>> GetUsersToTag(string searchQuery, int take, int skip, bool includeDeleted = false,
        bool shouldEnableTagging = true)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<User>> GetUsersToTagAsync(string searchQuery, int take, int skip, bool includeDeleted = false, bool shouldEnableTagging = true, CancellationToken token = default)
    {
        EntitySpecification<User> userListSpecification = new EntitySpecification<User>(x => 
            EF.Functions.ILike($"{x.EmailAddress} {x.NormalizedFullName}", $"%{searchQuery}%") &&
            shouldEnableTagging ? x.AccountSetting.AllowTagging == shouldEnableTagging : true
            );

        userListSpecification.Include(x => x.AccountSetting);
        var users = await _userRepository.ListAsync(skip, take, false, userListSpecification, token, includeDeleted);
        return users;
    }
}