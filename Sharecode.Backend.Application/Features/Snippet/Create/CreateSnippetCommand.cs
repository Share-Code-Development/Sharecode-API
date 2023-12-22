using Sharecode.Backend.Application.Base;

namespace Sharecode.Backend.Application.Features.Snippet.Create;

public class CreateSnippetCommand : IAppRequest<SnippetCreatedResponse>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Language { get; set; }
    public List<string> Tags { get; set; }
    public bool Public { get; set; }
    public byte[] Content { get; set; }
    public string PreviewCode { get; set; }
    public bool LimitComment { get; set; } = false;
    
    public CreateSnippetCommand(string title, string? description, string language, List<string> tags, bool isPublic, byte[]? content)
    {
        Title = title;
        Description = description;
        Language = language;
        Tags = tags;
        Public = isPublic;
        Content = content ?? new byte[0]; // assigns an empty array if content is null
    }
}