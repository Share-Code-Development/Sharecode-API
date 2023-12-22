using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Exceptions.Snippet;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Snippet.Get;

public class GetSnippetQueryHandler(IHttpClientContext clientContext, ISnippetService service, IFileClient fileClient) : IRequestHandler<GetSnippetQuery, GetSnippetResponse?>
{
    public async Task<GetSnippetResponse?> Handle(GetSnippetQuery request, CancellationToken cancellationToken)
    {
        var userId = await clientContext.GetUserIdentifierAsync();
        var aggregatedData = await service.GetAggregatedData(request.SnippetId, userId);
        if (aggregatedData == null)
            return null;

        var blob = await fileClient.GetFileAsStringAsync(request.SnippetId.ToString(), cancellationToken);
        var response = GetSnippetResponse.From(aggregatedData);
        if (blob == null)
        {
            //TODO handle this case
            return response;
        }

        response.Blob = blob;
        return response;
    }
}