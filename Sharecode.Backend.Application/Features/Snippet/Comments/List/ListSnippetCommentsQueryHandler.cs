using MediatR;

namespace Sharecode.Backend.Application.Features.Snippet.Comments.List;

public class ListSnippetCommentsQueryHandler() : IRequestHandler<ListSnippetCommentsQuery, ListSnippetCommentsResponse>
{
    public Task<ListSnippetCommentsResponse> Handle(ListSnippetCommentsQuery request, CancellationToken cancellationToken)
    {
        return null;
    }
}