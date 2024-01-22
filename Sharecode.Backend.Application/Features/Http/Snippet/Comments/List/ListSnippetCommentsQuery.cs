using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Application.Features.Http.Snippet.Comments.List;

public class ListSnippetCommentsQuery : ListQuery, IAppRequest<ListSnippetCommentsResponse?>
{
    public Guid SnippetId { get; set; }
    public Guid? ParentCommentId { get; set; }
    
    public override string DefaultOrderBy()
    {
        return string.Empty;
    }
}