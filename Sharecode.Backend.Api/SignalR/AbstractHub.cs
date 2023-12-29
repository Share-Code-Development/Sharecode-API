using Microsoft.AspNetCore.SignalR;
using Sharecode.Backend.Application.Client;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.SignalR;

public abstract class AbstractHub<TClient>(ILogger logger, IGroupStateManager groupStateManager) : Hub<TClient> where TClient : class
{

    protected IGroupStateManager StateManager => groupStateManager;
    
    protected async Task<bool> AddToGroupAsync(string groupName, string connectionId, string userIdentifier, CancellationToken token = default)
    {
        bool managedResponse = false;
        try
        {
            await Groups.AddToGroupAsync(connectionId, groupName, token);
            managedResponse = await groupStateManager.AddAsync(groupName, connectionId, userIdentifier, token);
            return managedResponse;
        }
        finally
        {
            if (!managedResponse)
            {
                await Groups.RemoveFromGroupAsync(connectionId, groupName, token);
                logger.Error("Failed to map connection {Connection} to the group {Group}", connectionId, groupName);
            }
        }
    }
    
    protected async Task<bool> DisconnectAsync(string connectionId, CancellationToken token = default)
    {
        try
        {
            var contextConnectionId = Context.ConnectionId;
            var groups = await StateManager.GetAllGroupsAsync(contextConnectionId, token);
            foreach (var group in groups)
            {
                await Groups.RemoveFromGroupAsync(contextConnectionId, group, Context.ConnectionAborted);
                await StateManager.RemoveAsync(group, contextConnectionId, Context.ConnectionAborted);
            }

            return false;
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to remove users {ConnectionId} from the group due to {Message}", Context.ConnectionId, e.Message);
            return false;
        }
        
    }
}