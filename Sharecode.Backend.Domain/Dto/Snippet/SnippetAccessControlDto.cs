namespace Sharecode.Backend.Domain.Dto.Snippet;

public class SnippetAccessControlDto
{
    public Guid UserId { get; set; }
    public bool Read { get; set; }
    public bool Write { get; set; }
    public bool Manage { get; set; }
}