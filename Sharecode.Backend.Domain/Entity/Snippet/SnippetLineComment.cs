using Sharecode.Backend.Domain.Entity.Interactions;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class SnippetLineComment : Comment
{
    public int LineNumber { get; set; }
    
    public Guid SnippetId { get; set; }
    
    public Snippet Snippet { get; set; }
}