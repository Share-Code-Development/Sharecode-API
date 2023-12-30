using Sharecode.Backend.Domain.Entity.Interactions;
using Sharecode.Backend.Domain.Events.Snippet.Comment;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class SnippetLineComment : Comment
{
    public int LineNumber { get; set; }
    
    public Guid SnippetId { get; set; }
    
    public Snippet Snippet { get; set; }

    public override void RaiseCreatedEvent()
    {
        RaiseDomainEvent(new SnippetLineCommentCreateEvent(SnippetId, UserId));
    }
}