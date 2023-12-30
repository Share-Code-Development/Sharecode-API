using MediatR;
using Sharecode.Backend.Domain.Events.Snippet.Comment;

namespace Sharecode.Backend.Worker.Outbox.Events.Snippet.Comment;

public class SnippetLineCommentCreatedHandler : INotificationHandler<SnippetLineCommentCreateEvent>
{
    public async Task Handle(SnippetLineCommentCreateEvent notification, CancellationToken cancellationToken)
    {
        return;
    }
}