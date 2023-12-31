﻿using MediatR;
using Microsoft.AspNetCore.SignalR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Live.Snippet;
using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Utilities.RedisCache;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.SignalR;

public class SnippetHub(ILogger logger, IGroupStateManager groupStateManager, IMediator mediator, IAppCacheClient appCacheClient) : AbstractHub<ISignalRClient>(logger, groupStateManager, mediator, appCacheClient)
{
    
    public override async Task OnConnectedAsync()
    {
        var queries = Context.GetHttpContext()?.Request.Query;
        if(queries == null)
            return;
        
        if (!queries.TryGetValue("snippetId", out var snippetIdRaw))
        {
            return;
        }
        
        if (!Guid.TryParse(snippetIdRaw, out var snippetId))
        {
            return;
        }

        var joinedSnippetEvent = new JoinedSnippetEvent()
        {
            SnippetId = snippetId
        };
        
        var joinedSnippetResponse = await Mediator.Send(joinedSnippetEvent);
        if (joinedSnippetResponse == null)
        {
            await Clients.Caller.Message(new LiveEvent<object>(new LiveEventConnectionRefused("You don't have access to this snippet")));
            Context.Abort();
            return;
        }

        var added = await AddToGroupAsync(joinedSnippetResponse.SnippetId.ToString(), Context.ConnectionId,
            joinedSnippetResponse.JoinedUserId.ToString() ?? joinedSnippetResponse.JoinedUserName);
        if (added)
            await Clients.Group(joinedSnippetResponse.SnippetId.ToString())
                .Message(LiveEvent<object>.Of(joinedSnippetEvent));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var contextConnectionId = Context.ConnectionId;
        if (exception != null)
        {
            logger.Error(exception, "A disconnect event has been called with an error message {Message} on connection id ", exception.Message, contextConnectionId);
        }
        
        await DisconnectAsync(Context.ConnectionId);
    }
    
    

    private async Task ShowMembers(Guid snippetId)
    {
        
    }
}