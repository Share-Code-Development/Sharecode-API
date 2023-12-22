using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Application.Features.Snippet.Create;

public class SnippetCreatedResponse()
{
    public Guid SnippetId { get; set; }
}