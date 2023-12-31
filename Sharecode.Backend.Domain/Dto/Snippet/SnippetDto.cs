using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Domain.Dto.Snippet;

public class SnippetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Language { get; set; }
    public string PreviewCode { get; set; }
    public List<string> Tags { get; set; }
    public bool Public { get; set; } = false;
    public long Views { get; set; }
    public long Copy { get; set; }
    public Guid? OwnerId { get; set; }
    public List<ReactionCommonDto> Reactions { get; set; } = [];
    public long CommentCount { get; set; } = 0;
    public string? Blob { get; set; }
    public List<SnippetAccessControlDto> AccessControl = [];
    public List<SnippetLineCommentDto> LineComments = [];
    public bool IsCommentsLimited = false;
}