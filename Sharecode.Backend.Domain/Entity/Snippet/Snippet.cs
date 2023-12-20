using System.ComponentModel.DataAnnotations;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Entity.Snippet;

public class Snippet : AggregateRootWithMetadata
{
    [Required]
    [Length(minimumLength: 2, maximumLength: 50)]
    public string Title { get; set; }
    [Required]
    [Length(minimumLength: 0, maximumLength: 500)]
    public string? Description { get; set; }
    [Required]
    public string Language { get; set; }
    [Required]
    [Length(minimumLength:0, maximumLength:200)]
    public string PreviewCode { get; set; }
    public List<string> Tags { get; private set; } = [];
    public bool Public { get; set; } = false;
    public long Views { get; set; }
    public long Copy { get; set; }
    
    public User? Owner { get; set; }
    public Guid? OwnerId { get; set; }

    public List<SnippetReactions> Reactions { get; private set; } = [];
    public List<SnippetComment> Comments { get; private set; } = [];
    public List<SnippetAccessControl> AccessControls { get; private set; } = [];
}