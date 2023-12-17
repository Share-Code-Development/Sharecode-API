using Sharecode.Backend.Domain.Entity.Interactions;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class SnippetCommentReactions : Reactions
{
    public SnippetComment SnippetComment { get; set; }
    public Guid SnippetCommentId { get; set; }
}