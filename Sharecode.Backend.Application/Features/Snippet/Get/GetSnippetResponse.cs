using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Application.Features.Snippet.Get;

public class GetSnippetResponse : SnippetDto
{
    public static GetSnippetResponse From(Domain.Entity.Snippet.Snippet snippet)
    {
        return new GetSnippetResponse()
        {
            Id = snippet.Id,
            Title = snippet.Title,
            Description = snippet.Description,
            Language = snippet.Language,
            PreviewCode = snippet.PreviewCode,
            Tags = snippet.Tags,
            Public = snippet.Public,
            Views = snippet.Views,
            Copy = snippet.Copy,
            OwnerId = snippet.OwnerId
        };
    }
    
    public static GetSnippetResponse From(SnippetDto snippet)
    {
        return new GetSnippetResponse()
        {
            Id = snippet.Id,
            Title = snippet.Title,
            Description = snippet.Description,
            Language = snippet.Language,
            PreviewCode = snippet.PreviewCode,
            Tags = snippet.Tags,
            Public = snippet.Public,
            Views = snippet.Views,
            Copy = snippet.Copy,
            OwnerId = snippet.OwnerId
        };
    }
}