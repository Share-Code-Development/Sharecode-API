using System.ComponentModel.DataAnnotations;
using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Application.Features.Snippet.Comments;

public class ListSnippetCommentsQuery : ListQuery, IAppRequest<ListSnippetCommentsResponse>
{
    public Guid SnippetId { get; set; }
    public Guid? ParentCommentId { get; set; }
}