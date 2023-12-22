using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Snippet.Get;

public class GetSnippetQuery : IAppRequest<GetSnippetResponse?>
{
    public Guid SnippetId { get; set; }
    public bool UpdateRecent { get; set; }
}