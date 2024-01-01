using System.Data;
using System.Transactions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Features.Http.Users.GetMySnippets;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Db;
using Sharecode.Backend.Infrastructure.Db.Extensions;
using Sharecode.Backend.Infrastructure.Exceptions;
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
    
    public async Task<bool> IsEmailAddressUniqueAsync(string emailAddress, CancellationToken token = default)
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

    public async Task<bool> RequestForgotPasswordAsync(string emailAddress, CancellationToken token = default)
    {
        var user = await _userRepository.GetAsync(emailAddress, true, token, true);
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
        var user = await _userRepository.GetAsync(userId, true, token);
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
        if(connectionContext == null)
            throw new InfrastructureDownException("Failed to get the users",
                $"Failed to create the dapper context for user search");
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

    public async Task<List<MySnippetsDto>> ListUserSnippetsAsync(Guid userId, bool onlyOwned = false,
        bool recentSnippets = true, int skip = 0, int take = 20,
        string order = "ASC", string orderBy = "ModifiedAt", string searchQuery = null, CancellationToken cancellationToken = default)
    {
        using var connectionContext = _userRepository.CreateDapperContext();
        if (connectionContext == null)
            throw new InfrastructureDownException($"Failed to list the user's " + (recentSnippets ? "recent " : "") + "snippets", $"Failed to create the dapper context for user search");
        List<MySnippetsDto> response = [];
        long totalCount = 0;
        connectionContext.Open();
        try
        {
            var transaction = connectionContext.BeginTransaction();

            var cursors = await ((NpgsqlConnection) connectionContext).QueryRefcursorsAsync((NpgsqlTransaction)transaction, UserSqlQueries.FunctionListUsersSnippet, CommandType.Text, new
            {
                userid = userId,
                onlyowned = onlyOwned,
                recent = recentSnippets,
                skip = skip,
                take = take,
                order = order, 
                orderby = orderBy,
                searchquery = searchQuery
            });

            var snippets = await cursors.ReadAsync<MySnippetsDto>();
            snippets = snippets.ToList();

            var reactions = await cursors.ReadAsync<SnippetsReactionDto>();
            reactions = reactions.ToList();
            
            foreach (var snippet in snippets)
            {
                var snippetReactionsList = reactions.Where(x => x.SnippetId == snippet.Id)
                    .Select(x => new ReactionCommonDto()
                    {
                        Reactions = x.Reactions,
                        ReactionType = x.ReactionType
                    })
                    .ToList();
                snippet.Reactions = snippetReactionsList;
                response.Add(snippet);
            }
        }
        finally
        {
            connectionContext.Close();
        }

        return (response);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, Guid requestedBy, bool softDelete = true, CancellationToken token = default)
    {
        var userToDelete = await _userRepository.GetAsync(userId, true, token);
        if (userToDelete == null)
            throw new EntityNotFoundException(typeof(User), userId, false);

        if (userToDelete.IsDeleted)
            return false;
        
        userToDelete.RequestAccountDeletion(softDelete, requestedBy);
        return true;
    }

    public async Task<Dictionary<string, object?>> UpdateExternalMetadataAsync(Guid userId, Dictionary<string, object> metadataValues, CancellationToken token = default)
    {
        var user = await _userRepository.GetAsync(userId, token: token);
        if (user == null)
            throw new EntityNotFoundException(typeof(User), userId);

        Dictionary<string, object?> oldValues = new();
        
        foreach (var (key, value) in metadataValues)
        {
            var val = user.SetMeta(key, value);
            oldValues[key] = val;
        }

        _userRepository.Update(user);
        return oldValues;
    }

    public async Task DeleteExternalMetadataAsync(Guid userId, HashSet<string> keys, CancellationToken token = default)
    {
        var user = await _userRepository.GetAsync(userId, token: token);
        if (user == null)
            throw new EntityNotFoundException(typeof(User), userId);
        
        user.DeleteMeta(keys);
        _userRepository.Update(user);
    }
}

internal static class UserSqlQueries
{
    public static string FunctionGetUsersToTag => $"SELECT \"EmailAddress\", \"FirstName\", \"MiddleName\", \"LastName\", \"Id\", \"ProfilePicture\" FROM get_users_to_tag(@searchquery::character varying, @skip, @take, @onlyenabled, @includedeleted)";

    public static string FunctionListUsersSnippet =>
        "SELECT * FROM list_user_snippets(@userid, @onlyowned, @recent, @skip, @take, @order, @orderby, @searchQuery);";
}