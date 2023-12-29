using MediatR;

namespace Sharecode.Backend.Application.Features.Live.Snippet;

public class JoinedSnippetLiveHandler : IRequestHandler<JoinedSnippetEvent, JoinedSnippetResponse?>
{
    public Task<JoinedSnippetResponse?> Handle(JoinedSnippetEvent request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}