using System.Data;
using Dapper;
using Npgsql;
using Serilog;
using Sharecode.Backend.Application;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Helper;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Db.Extensions;
using Sharecode.Backend.Infrastructure.Exceptions;

namespace Sharecode.Backend.Infrastructure.Service;

public class SnippetService(
    ISnippetRepository snippetRepository, 
    ISnippetLineCommentRepository lineCommentRepository,
    ISnippetCommentRepository commentRepository,
    ISnippetAccessControlRepository accessControlRepository,
    ISnippetReactionRepository snippetReactionRepository,
    ISnippetCommentReactionRepository snippetCommentReactionRepository,
    ILogger logger) : ISnippetService
{
    public async Task<SnippetDto?> GetSnippet(Guid snippetId, Guid? requestedUser, bool updateRecent = false,
        bool updateView = false)
    {
        using var dapperContext = snippetRepository.CreateDapperContext();
        if (dapperContext == null)
            throw new InfrastructureDownException("Failed to get the snippet",
                $"Failed to create the dapper context for aggregated data");
        
        dapperContext.Open();
        try
        {
            using var transaction = dapperContext.BeginTransaction();
            var cursor = await ((NpgsqlConnection)dapperContext).QueryRefcursorsAsync((NpgsqlTransaction)transaction, $"SELECT * FROM get_snippet(@snippetid, @requestedby, @updaterecent, @updateview)", CommandType.Text,new { snippetid = snippetId, requestedby = requestedUser, updaterecent = updateRecent, updateview = updateView });

            var snippetDto = cursor.ReadSingleOrDefault<SnippetDto>();
            if (snippetDto == null)
                return null;
        
            var totalCommentCountData = cursor.ReadFirst<dynamic>();
            var lineCommentData = cursor.Read<SnippetLineCommentDto>().ToList();
            var reactionsData = cursor.Read<ReactionCommonDto>().ToList();
            var accessControlsData = cursor.Read<SnippetAccessControlDto>().ToList();
            
            snippetDto.CommentCount = ((int) totalCommentCountData.count);
            snippetDto.AccessControl = accessControlsData;
            snippetDto.Reactions = reactionsData;
            snippetDto.LineComments = lineCommentData;
            return snippetDto;
        }
        catch (Exception e)
        {
            if (e is PostgresException { MessageText: "No Access" })
            {
                throw new NoSnippetAccessException(snippetId);
            }
            
            logger.Error(e, "Failed to get the snippet of Id {Id} due to {Message}", snippetId, e.Message);
            throw;
        }
        finally
        {
            dapperContext.Close();
        }
    }
    
    public async Task<SnippetAccessPermission> GetSnippetAccess(Guid snippetId, Guid requestedUser, bool checkAdminAccess = true)
    {
        try
        {
            using var dapperContext = snippetRepository.CreateDapperContext();
            if (dapperContext == null)
                throw new InfrastructureDownException("Failed to get snippet permissions",
                    "Failed to create dapper context for getting snippet permission");

            var param = new DynamicParameters();
            param.Add("snippetId", snippetId);
            param.Add("requestedUser", requestedUser);
            param.Add("checkAdminPermission", checkAdminAccess);

            var permission = await dapperContext.QueryFirstOrDefaultAsync<SnippetAccessPermission>(SnippetUserSqlQueries.GetSnippetAccess, param, commandTimeout: 1000);
            if(permission == null)
                permission = SnippetAccessPermission.NoPermission(snippetId, requestedUser);
            
            return permission;
        }
        catch (Exception e)
        {
            if (e is InfrastructureDownException)
                throw;
            
            logger.Error(e, "An unknown error occured while fetching the permission of user {User} on Snippet {Snippet} due to {Message}", requestedUser, snippetId, e.Message);
            return SnippetAccessPermission.Error;
        }
    }

    public async Task<bool> DeleteSnippet(Guid snippedId, Guid requestedBy)
    {
        try
        {
            var accessControlSpecification = new EntitySpecification<SnippetAccessControl>(x => x.SnippetId == snippedId);
            await accessControlRepository.DeleteBatchAsync(accessControlSpecification);

            var snippetReactionSpecification = new EntitySpecification<SnippetReactions>(x => x.SnippetId == snippedId);
            await snippetReactionRepository.DeleteBatchAsync(snippetReactionSpecification);

            var snippetLineCommentSpecification = new EntitySpecification<SnippetLineComment>(x => x.SnippetId == snippedId);
            await lineCommentRepository.DeleteBatchAsync(snippetLineCommentSpecification);

            var snippetCommentReactionSpecification = new EntitySpecification<SnippetCommentReactions>(x => x.SnippetComment.SnippetId == snippedId);
            await snippetCommentReactionRepository.DeleteBatchAsync(snippetCommentReactionSpecification);

            var snippetCommentSpecification = new EntitySpecification<SnippetComment>(x => x.SnippetId == snippedId);
            await commentRepository.DeleteBatchAsync(snippetCommentSpecification);
        
            snippetRepository.Delete(snippedId);
            return true;
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to delete the snippet of {SnippetId} by {Requester} due to {Error}", snippedId, requestedBy ,e.Message);
            return false;
        }
    }

    public async Task<HashSet<string>> GetUsersReactions(Guid snippetId, Guid requestedUser, CancellationToken token = default)
    {
        var reactions = await snippetReactionRepository.GetReactionsOfUser(snippetId, requestedUser, token);
        return reactions
            .Select(x => x.ReactionType)
            .ToHashSet();
    }
}

internal sealed class SnippetUserSqlQueries
{
    public static string GetSnippetAccess =>
        $"SELECT \"Read\", \"Write\", \"Manage\" FROM get_permission_of_snippet(@snippetId, @requestedUser, @checkAdminPermission)";
}