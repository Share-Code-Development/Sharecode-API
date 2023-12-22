namespace Sharecode.Backend.Domain.Events.Snippet.Comment;

public record SnippetCommentCreateEvent(
    Guid CommentId,
    Guid SnippetId,
    Guid Owner
    ) : SnippetCommentEvent(SnippetId, Owner);