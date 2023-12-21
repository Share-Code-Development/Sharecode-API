using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Service;    
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Utilities.SecurityClient;
using CommandDefinition = Dapper.CommandDefinition;

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

    public async Task<IReadOnlyList<User>> GetUsersToTagAsync(string searchQuery, int take, int skip, bool includeDeleted = false, bool shouldEnableTagging = true, CancellationToken token = default)
    {
        using var connectionContext = _userRepository.CreateDapperContext();
        var functionParams = new DynamicParameters();
        functionParams.Add("searchquery", searchQuery, dbType: (DbType?)NpgsqlDbType.Varchar);
        functionParams.Add("skip", skip);
        functionParams.Add("take", take);
        functionParams.Add("onlyenabled", shouldEnableTagging);
        functionParams.Add("includedeleted", includeDeleted);
        var mentionableUsers = await connectionContext.QueryAsync<User>(
            UserSqlQueries.FunctionGetUsersToTag, functionParams, commandTimeout: 1000);
        return mentionableUsers.ToList();
    }
}

internal static class UserSqlQueries
{
    public static string FunctionGetUsersToTag => $"SELECT \"EmailAddress\", \"FirstName\", \"MiddleName\", \"LastName\", \"Id\", \"ProfilePicture\" FROM get_users_to_tag(@searchquery::character varying, @skip, @take, @onlyenabled, @includedeleted)";
}