using Sharecode.Backend.Domain.Base;

namespace Sharecode.Backend.Domain.Events.Snippet.Comment;

public record SnippetCommentEvent(
    Guid SnippetId,
    Guid Owner
    ) : IDomainEvent;