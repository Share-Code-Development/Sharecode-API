using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Application.Features.Http.Snippet.Get;

public class GetSnippetQueryHandler(IHttpClientContext clientContext, ISnippetService service, IFileClient fileClient) : IRequestHandler<GetSnippetQuery, GetSnippetResponse?>
{
    public async Task<GetSnippetResponse?> Handle(GetSnippetQuery request, CancellationToken cancellationToken)
    {
        var userId = await clientContext.GetUserIdentifierAsync();
        var aggregatedData = await service.GetSnippet(request.SnippetId, userId, request.UpdateRecent, request.UpdateView);
        if (aggregatedData == null)
            throw new EntityNotFoundException(typeof(Domain.Entity.Snippet.Snippet), request.SnippetId, true); 

        var blob = await fileClient.GetFileAsStringAsync(request.SnippetId.ToString(), cancellationToken);
        var response = GetSnippetResponse.From(aggregatedData);
        if (blob == null)
        {
            throw new EntityNotFoundException(typeof(Domain.Entity.Snippet.Snippet), request.SnippetId, true);
        }

        if (userId.HasValue && request.UpdateRecent)
        {
            clientContext.AddCacheKeyToInvalidate(CacheModules.UserSnippet, userId.Value.ToString(), "recent");
        }
        response.Blob = blob;
        return response;
    }
}