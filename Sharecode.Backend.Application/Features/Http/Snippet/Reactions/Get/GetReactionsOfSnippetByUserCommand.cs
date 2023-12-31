using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Http.Snippet.Reactions.Get;

public class GetReactionsOfSnippetByUserCommand : IAppRequest<GetReactionsOfSnippetByUserResponse>
{
    public Guid SnippetId { get; set; }
    public Guid UserId { get; set; }
}