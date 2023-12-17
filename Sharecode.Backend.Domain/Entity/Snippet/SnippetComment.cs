using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Interactions;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class SnippetComment : Comment
{
    public Snippet Snippet { get; set; }
    public Guid SnippetId { get; set; }
    public List<SnippetCommentReactions> Reactions { get; set; }
    public Guid? ParentCommentId { get; set; }
    public SnippetComment? ParentComment { get; set; }
}