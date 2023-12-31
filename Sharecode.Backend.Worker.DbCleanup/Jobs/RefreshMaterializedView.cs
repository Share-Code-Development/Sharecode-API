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
        logger.Information("Starting to update MV_SnippetReactions");
        try
        {
            await dbContext.ExecuteAsync(async x => 
            {
                x.CommandText = $"REFRESH MATERIALIZED VIEW CONCURRENTLY snippet.\"MV_SnippetReactions\"";
                var queryCount = await x.ExecuteNonQueryAsync();
                return queryCount > 0;
            });
            
            logger.Information("The view has been updated with latest result");
        }
        catch (Exception e)
        {
            logger.Error("Failed to update the statistics count due to {Message}", e.Message);
        }
    }
}