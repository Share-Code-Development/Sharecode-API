namespace Sharecode.Backend.Domain.Dto.Snippet;

public class ReactionCommonDto
{
    public string ReactionType { get; set; }
    public int Reactions { get; set; }
}

public class SnippetsReactionDto : ReactionCommonDto
{
    public Guid SnippetId { get; set; }
}