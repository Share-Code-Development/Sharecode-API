using Sharecode.Backend.Domain.Entity.Interactions;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class SnippetReactions : Reactions
{
    public Snippet Snippet { get; set; }
    public Guid SnippetId { get; set; }
}