using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Snippet.Get;

public class GetSnippetQueryHandler(IHttpClientContext clientContext, ISnippetRepository repository) : IRequestHandler<GetSnippetQuery, GetSnippetResponse?>
{
    public async Task<GetSnippetResponse?> Handle(GetSnippetQuery request, CancellationToken cancellationToken)
    {
        var snippet = await repository.GetSnippetById(request.SnippetId);

        return null;
    }
}