using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Event;
using Sharecode.Backend.Application.Event.Outbond;
using Sharecode.Backend.Application.Features.Live.Snippet;
using Sharecode.Backend.Application.Features.Live.Snippet.Joined;
using Sharecode.Backend.Application.Features.Live.Snippet.Left;
using Sharecode.Backend.Application.LiveEvents.Inbound;
using Sharecode.Backend.Application.Models;
using Sharecode.Backend.Domain.Base.Interfaces;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Utilities.RedisCache;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.SignalR;

public class SnippetHub(ILogger logger, IGroupStateManager groupStateManager, IMediator mediator, IAppCacheClient appCacheClient) : AbstractHub(logger, groupStateManager, mediator, appCacheClient)
{

    static SnippetHub()
    {
        RegisterNonReturn("start_typing", (data, context) =>
        {
            try
            {
                var dataRequest = data.ToString() ?? string.Empty;
                if (string.IsNullOrEmpty(dataRequest))
                    return Task.CompletedTask;
                
                var lineCommentTypingEvent =
                    JsonConvert.DeserializeObject<LineCommentTypingEvent>(dataRequest);
                if(lineCommentTypingEvent == null)
                    return Task.CompletedTask;

                var name = context.HubCallerContext.Items["NAME"]?.ToString();
                var tryParse = Guid.TryParse(context.HubCallerContext.Items["IDENTIFIER"]?.ToString(), out var parsed);
                var @event = new LineCommentTypingNotificationEvent()
                {
                    Action = lineCommentTypingEvent.Action,
                    LineNumber = lineCommentTypingEvent.LineNumber,
                    UserName = name ?? string.Empty,
                    UserIdentifier = tryParse ? parsed : Guid.Empty
                };
                
                context.Clients.OthersInGroup(lineCommentTypingEvent.SnippetId.ToString())
                    .Message(LiveEvent<object>.Of(@event));
            }
            catch (Exception e)
            {
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        });
        
        
    }
    
    private readonly ILogger _logger = logger;
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
            SnippetId = snippetId,
            ConnectionId = Context.ConnectionId
        };
        
        var joinedSnippetResponse = await Mediator.Send(joinedSnippetEvent);
        if (joinedSnippetResponse == null)
        {
            await Clients.Caller.Message(new LiveEvent<object>(new LiveEventConnectionRefused("You don't have access to this snippet")));
            Context.Abort();
            return;
        }

        var added = await AddToGroupAsync(joinedSnippetResponse.SnippetId.ToString(), Context.ConnectionId,
            (joinedSnippetResponse.JoinedUserId.GetValueOrDefault() == Guid.Empty) ? joinedSnippetResponse.JoinedUserName : joinedSnippetResponse.JoinedUserId!.Value.ToString());
        if (added)
        {
            await Clients.Group(joinedSnippetResponse.SnippetId.ToString())
                .Message(LiveEvent<object>.Of(joinedSnippetResponse));
            Context.Items["NAME"] = joinedSnippetResponse.JoinedUserName;
            Context.Items["IDENTIFIER"] = joinedSnippetResponse.JoinedUserId.GetValueOrDefault();
        }
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var contextConnectionId = Context.ConnectionId;
        if (exception != null)
        {
            _logger.Error(exception, "A disconnect event has been called with an error message {Message} on connection id ", exception.Message, contextConnectionId);
        }

        HashSet<string> groupsAsync = await groupStateManager.GetAllGroupsAsync(contextConnectionId);
        await DisconnectAsync(Context.ConnectionId);
        LeftSnippetResponse response = new LeftSnippetResponse();
        response.LeftUserName = Context.Items["NAME"]?.ToString() ?? string.Empty;
        if (Guid.TryParse(Context.Items["IDENTIFIER"]?.ToString() ?? string.Empty, out var parsedId))
        {
            response.LeftUserId = parsedId;
        }
        var liveEvent = LiveEvent<object>.Of(response);
        await Clients.Groups(groupsAsync).Message(liveEvent);
    }
}