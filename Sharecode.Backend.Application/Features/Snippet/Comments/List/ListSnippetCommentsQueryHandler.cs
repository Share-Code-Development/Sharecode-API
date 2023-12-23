using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Service;

namespace Sharecode.Backend.Application.Features.Snippet.Comments.List;

public class ListSnippetCommentsQueryHandler(ISnippetCommentService commentService, ISnippetService snippetService, IHttpClientContext clientContext, ILogger logger) : IRequestHandler<ListSnippetCommentsQuery, ListSnippetCommentsResponse>
{
    public Task<ListSnippetCommentsResponse> Handle(ListSnippetCommentsQuery request, CancellationToken cancellationToken)
    {
        return null;
    }
}