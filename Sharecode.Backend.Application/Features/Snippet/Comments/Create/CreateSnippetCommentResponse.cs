namespace Sharecode.Backend.Application.Features.Snippet.Comments.Create;

public class CreateSnippetCommentResponse
{
    public Guid Id { get; set; }
    public Guid SnippetId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public int? LineNumber { get; set; }
}