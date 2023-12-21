

using System.Diagnostics;
using Sharecode.Backend.Domain.Entity.Interactions;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class SnippetAccessControl : AccessControl<Snippet>
{
    public Guid SnippetId {get; set; }
    public Snippet Snippet { get; set; }
    public override void SetOwnership(Snippet entity)
    {
        SnippetId = entity.Id;
        CreateForOwner(entity.OwnerId!.Value);
    }
}