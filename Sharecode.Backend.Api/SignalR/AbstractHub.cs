using MediatR;
using Microsoft.AspNetCore.SignalR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Event;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Utilities.RedisCache;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.SignalR;

public abstract class AbstractHub(ILogger logger, IGroupStateManager groupStateManager, IMediator mediator ,IAppCacheClient cacheClient) : Hub<ISignalRClient>
{
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

    
    public async Task Execute(ClientEvent @event)
    {
        if(string.IsNullOrEmpty(@event.EventType))
        {
            throw new LiveException("No event type specified");
        }

        var action = TriggerFunctions[@event.EventType];
        if (action == null)
        {
            throw new LiveException("No event action has been registered", eventType: @event.EventType);
        }

        try
        {
            await action(@event.Event, new LiveEventContext(Context, Clients, groupStateManager));
        }
        catch (Exception e)
        {
            logger.Error(e, "An unknown error occured while handling the Invoke of {FunctionName} by {CallerId}", @event.EventType, Context.ConnectionId);
            throw new LiveException("An error has been occured while processing", extendedMessage: e.Message, eventType: @event.EventType);
        }
    }
    
    public async Task<LiveEvent<object>> Invoke(ClientEvent @event)
    {
        if(string.IsNullOrEmpty(@event.EventType))
        {
            throw new LiveException("No event type specified");
        }

        var action = InvokeFunctions[@event.EventType];
        if (action == null)
        {
            throw new LiveException("No event action has been registered", eventType: @event.EventType);
        }
        
        try
        {
            return await action(@event.Event, new LiveEventContext(Context, Clients, groupStateManager));
        }
        catch (Exception e)
        {
            logger.Error(e, "An unknown error occured while handling the Invoke of {FunctionName} by {CallerId}", @event.EventType, Context.ConnectionId);
            throw new LiveException("An error has been occured while processing", extendedMessage: e.Message, eventType: @event.EventType);
        }
    }

    #region Static Invoke Functions

    private static readonly Dictionary<string, Func<object, LiveEventContext, Task<LiveEvent<object>>>> InvokeFunctions = new();
    private static readonly Dictionary<string, Func<object, LiveEventContext, Task>> TriggerFunctions = new();
    
    protected static void RegisterReturn(string eventType, Func<object, LiveEventContext, Task<LiveEvent<object>>> func)
    {
        InvokeFunctions[eventType] = func;
    }
    
    protected static void RegisterNonReturn(string eventType, Func<object, LiveEventContext, Task> act)
    {
        TriggerFunctions[eventType] = act;
    }

    #endregion
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