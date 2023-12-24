namespace Sharecode.Backend.Domain.Events.Snippet.Comment;

public record SnippetReplyCommentCreateEvent(Guid CommentId, Guid SnippetId, Guid Owner, Guid ParentId) : SnippetCommentCreateEvent(CommentId, SnippetId, Owner)
{
    
}