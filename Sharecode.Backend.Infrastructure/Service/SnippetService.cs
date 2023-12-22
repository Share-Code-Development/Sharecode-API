using System.ComponentModel;
using System.Data;
using Npgsql;
using Serilog;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Infrastructure.Db.Extensions;
using Sharecode.Backend.Infrastructure.Exceptions;

namespace Sharecode.Backend.Infrastructure.Service;

public class SnippetService(ISnippetRepository snippetRepository, ILogger logger) : ISnippetService
{
    public async Task<SnippetDto?> GetAggregatedData(Guid snippetId, Guid? requestedUser)
    {
        using var dapperContext = snippetRepository.CreateDapperContext();
        if (dapperContext == null)
            throw new InfrastructureDownException("Failed to get the snippet",
                $"Failed to create the dapper context for aggregated data");
        
        dapperContext.Open();
        try
        {
            using var transaction = dapperContext.BeginTransaction();
            var cursor = await ((NpgsqlConnection)dapperContext).QueryRefcursorsAsync((NpgsqlTransaction)transaction, $"SELECT * FROM get_snippet(@snippetid, @requestedby)", CommandType.Text,new { snippetid = snippetId, requestedby = requestedUser });

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
}