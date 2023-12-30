

using System.Diagnostics;
using Sharecode.Backend.Domain.Attributes;
using Sharecode.Backend.Domain.Entity.Interactions;

namespace Sharecode.Backend.Domain.Entity.Snippet;

[HardDelete]
public class SnippetAccessControl : AccessControl<Snippet>
{
    public Guid SnippetId {get; set; }
    public Snippet Snippet { get; set; }
    public override void SetOwnership(Snippet entity)
    {
        SnippetId = entity.Id;
        CreateForOwner(entity.OwnerId!.Value);
    }

    public override void SoftDeleteEntity()
    {
        HardDeleteEntity();
    }
}