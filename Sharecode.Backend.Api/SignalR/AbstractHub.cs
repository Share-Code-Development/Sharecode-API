using MediatR;
using Microsoft.AspNetCore.SignalR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Utilities.RedisCache;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.SignalR;

public abstract class AbstractHub<TClient>(ILogger logger, IGroupStateManager groupStateManager, IMediator mediator ,IAppCacheClient cacheClient) : Hub<TClient> where TClient : class
{
    private static readonly Dictionary<string, Func<object, LiveEventContext, Task<LiveEvent<object>>>> InvokeFunc = new();
    private static readonly Dictionary<string, Func<object, LiveEventContext, Task>> SendFunc = new();
    protected IAppCacheClient CacheClient => cacheClient;
    protected IMediator Mediator => mediator;
    private IGroupStateManager StateManager => groupStateManager;
    
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

    protected static void RegisterReturn(string eventType, Func<object, LiveEventContext, Task<LiveEvent<object>>> func)
    {
        InvokeFunc[eventType] = func;
    }
    
    protected static void RegisterNonReturn(string eventType, Func<object, LiveEventContext, Task> act)
    {
        SendFunc[eventType] = act;
    }
    protected static Func<object, LiveEventContext, Task>? Action(string type) => SendFunc[type];
    protected static Func<object, LiveEventContext, Task<LiveEvent<object>>>? Invoke(string type) => InvokeFunc[type];
    
    
}

public sealed class LiveEventContext
{
    public LiveEventContext(HubCallerContext hubCallerContext, IHubCallerClients<ISignalRClient> clients, IGroupStateManager stateManager)
    {
        HubCallerContext = hubCallerContext;
        Clients = clients;
        StateManager = stateManager;
    }
    public IHubCallerClients<ISignalRClient> Clients { get; }
    public HubCallerContext HubCallerContext { get; }
    
    public IGroupStateManager StateManager { get; }
}