using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Http.Snippet.Comments.Create;

public class CreateSnippetCommentCommand : IAppRequest<CreateSnippetCommentResponse>
{
    public SnippetCommentType CommentType { get; set; }
    public Guid SnippetId { get; set; }
    public bool NotifyReplies { get; set; }
    public string Text { get; set; }
    public Guid? ParentCommentId { get; set; }
    public int? LineNumber { get; set; }
}