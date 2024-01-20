using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Live.Snippet;

public class JoinedSnippetEvent : IAppRequest<JoinedSnippetResponse?>
{
    public Guid SnippetId { get; init; }
    public string ConnectionId { get; init; }
}