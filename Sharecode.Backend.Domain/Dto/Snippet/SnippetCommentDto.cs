namespace Sharecode.Backend.Domain.Dto.Snippet;

public class SnippetCommentDto
{
    public Guid UserId { get; set; }
    public string? ProfilePicture { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    public List<MentionDto> Mentions { get; set; } = [];
    public bool HasReplies { get; set; } = false;
    public Dictionary<string, long> Reactions { get; set; } = new();
}