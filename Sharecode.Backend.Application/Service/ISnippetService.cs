using Sharecode.Backend.Domain.Dto.Snippet;

namespace Sharecode.Backend.Application.Service;

public interface ISnippetService
{
    Task<SnippetDto?> GetAggregatedData(Guid snippetId);
}