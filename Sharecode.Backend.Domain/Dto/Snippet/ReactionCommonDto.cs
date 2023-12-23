namespace Sharecode.Backend.Domain.Dto.Snippet;

public class ReactionCommonDto
{
    public string ReactionType { get; set; }
    public int Count { get; set; }
}

public class SnippetsReactionDto : ReactionCommonDto
{
    public Guid SnippetId { get; set; }
}