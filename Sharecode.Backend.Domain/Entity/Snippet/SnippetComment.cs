using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Interactions;
using Sharecode.Backend.Domain.Events.Snippet.Comment;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class SnippetComment : Comment
{
    public Snippet Snippet { get; set; }
    public Guid SnippetId { get; set; }
    public List<SnippetCommentReactions> Reactions { get; set; }
    public Guid? ParentCommentId { get; set; }
    public SnippetComment? ParentComment { get; set; }

    public override void RaiseCreatedEvent()
    {
        RaiseDomainEvent(new SnippetCommentCreateEvent(Id, SnippetId, UserId));
    }
}