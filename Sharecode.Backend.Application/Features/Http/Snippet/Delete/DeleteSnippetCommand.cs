using Sharecode.Backend.Application.Base;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Application.Features.Http.Snippet.Delete;

public class DeleteSnippetCommand : IAppRequest<DeleteResponse>
{
    public Guid SnippetId { get; init; }
}