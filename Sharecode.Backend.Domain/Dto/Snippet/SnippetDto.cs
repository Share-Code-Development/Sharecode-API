using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Domain.Dto.Snippet;

public class SnippetDto
{
    public Guid SnippetId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Language { get; set; }
    public string PreviewCode { get; set; }
    public List<string> Tags { get; set; }
    public bool Public { get; set; } = false;
    public long Views { get; set; }
    public long Copy { get; set; }
    public Guid? OwnerId { get; set; }
    public List<SnippetReactions> Reactions { get; private set; } = [];
    public List<SnippetComment> Comments { get; private set; } = [];

    public SnippetDto(Guid snippetId, string title, string? description, string language, string previewCode, List<string> tags, bool @public, long views, long copy, Guid? ownerId, List<SnippetReactions> reactions, List<SnippetComment> comments)
    {
        SnippetId = snippetId;
        Title = title;
        Description = description;
        Language = language;
        PreviewCode = previewCode;
        Tags = tags;
        Public = @public;
        Views = views;
        Copy = copy;
        OwnerId = ownerId;
        Reactions = reactions;
        Comments = comments;
    }

    public static SnippetDto From(Sharecode.Backend.Domain.Entity.Snippet.Snippet snippet)
    {
        return new(
            snippet.Id,
             snippet.Title,
            snippet.Description,
            snippet.Language,
            snippet.PreviewCode,
            snippet.Tags ?? [],
            snippet.Public,
            snippet.Views,
            snippet.Copy,
            snippet.OwnerId,
            snippet.Reactions ?? [],
            snippet.Comments ?? []
        );
    }
}