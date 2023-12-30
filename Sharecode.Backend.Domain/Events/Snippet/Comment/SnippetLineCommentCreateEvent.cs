namespace Sharecode.Backend.Domain.Events.Snippet.Comment;

public record SnippetLineCommentCreateEvent(Guid SnippetId, Guid Owner) : SnippetCommentEvent(SnippetId, Owner)
{
    
}