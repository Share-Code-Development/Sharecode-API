using Quartz;
using Sharecode.Backend.Infrastructure.Exceptions;
using Sharecode.Backend.Worker.DbCleanup.Application;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Worker.DbCleanup.Jobs;

[DisallowConcurrentExecution]
public class RefreshMaterializedView(ILogger logger, IPgSqlDbContext dbContext) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {

        await RefreshSnippetReactions();
        await RefreshSnippetCommentReactions();

    }
    private async Task RefreshSnippetReactions()
    {
        try
        {
            logger.Information("Starting to update MV_SnippetReactions");
            await dbContext.ExecuteAsync(async x => 
            {
                x.CommandText = $"REFRESH MATERIALIZED VIEW CONCURRENTLY snippet.\"MV_SnippetReactions\"";
                var queryCount = await x.ExecuteNonQueryAsync();
                return queryCount > 0;
            });
            
            logger.Information("The view has been updated with latest result of MV_SnippetReactions");
        }
        catch (Exception e)
        {
            logger.Error("Failed to update the statistics count of MV_SnippetReactions due to {Message}", e.Message);
        }
    }

    private async Task RefreshSnippetCommentReactions()
    {
        try
        {
            logger.Information("Starting to update MV_SnippetCommentReactions");
            await dbContext.ExecuteAsync(async x =>
            {
                x.CommandText = $"REFRESH MATERIALIZED VIEW CONCURRENTLY snippet.\"MV_SnippetCommentReactions\"";
                var queryCount = await x.ExecuteNonQueryAsync();
                return queryCount > 0;
            });
            
            logger.Information("The view has been updated with latest result of MV_SnippetCommentReactions");
        }
        catch (Exception e)
        {
            logger.Error("Failed to update the statistics count of MV_SnippetCommentReactions due to {Message}", e.Message);
        }
    }
}