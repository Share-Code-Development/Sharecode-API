using Sharecode.Backend.Domain.Dto.Snippet;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Application.Features.Snippet.Get;

public class GetSnippetResponse : SnippetDto
{
    public GetSnippetResponse(Guid snippetId, string title, string? description, string language, string previewCode, List<string> tags, bool @public, long views, long copy, Guid? ownerId, List<SnippetReactions> reactions, List<SnippetComment> comments) : base(snippetId, title, description, language, previewCode, tags, @public, views, copy, ownerId, reactions, comments)
    {
    }
}