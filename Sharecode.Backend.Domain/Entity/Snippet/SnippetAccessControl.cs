

using Sharecode.Backend.Domain.Entity.Interactions;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class SnippetAccessControl : AccessControl
{
    public Guid SnippetId {get; set; }
    public Snippet Snippet { get; set; }
}