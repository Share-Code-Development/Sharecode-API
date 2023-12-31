using MediatR;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Domain.Exceptions;

namespace Sharecode.Backend.Application.Features.Http.Snippet.Reactions.Get;

public class GetReactionOfSnippetByUserHandler(ISnippetService snippetService, IHttpClientContext context) : IRequestHandler<GetReactionsOfSnippetByUserCommand, GetReactionsOfSnippetByUserResponse>
{
    public async Task<GetReactionsOfSnippetByUserResponse> Handle(GetReactionsOfSnippetByUserCommand request, CancellationToken cancellationToken)
    {
        var userIdRaw = await context.GetUserIdentifierAsync();
        if (!userIdRaw.HasValue)
            throw new NoAccessException($"{request.SnippetId.ToString()}/{request.UserId.ToString()}", Guid.Empty, typeof(SnippetReactions));

        var userId = userIdRaw.Value;
        if (userId != request.UserId)
        {
            if (!await context.HasPermissionAsync(Permissions.ViewSnippetOthersAdmin, cancellationToken))
            {
                throw new NotEnoughPermissionException("Reaction of a user's snippet");
            }
        }

        var usersReactions = await snippetService
            .GetUsersReactions(request.SnippetId, request.UserId, cancellationToken);
        
        return GetReactionsOfSnippetByUserResponse.From(usersReactions);
    }
}