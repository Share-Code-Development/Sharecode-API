using Sharecode.Backend.Domain.Dto.Snippet;

namespace Sharecode.Backend.Application.Features.Live.Snippet.Joined;

public class JoinedSnippetResponse
{
    public Guid SnippetId { get; init; }
    public Guid? JoinedUserId { get; set; }
    public string JoinedUserName { get; set; }
    public SnippetAccessControlDto JoinedUserAccesses { get; set; }
    public List<ActiveSnippetUsersDto> ActiveUsers { get; set;  } = [];
}

public sealed record ActiveSnippetUsersDto
{
    public Guid? Id { get; init; }
    public string ConnectionId { get; set; }
    public string FullName { get; init; }
    public string? ProfilePicture { get; init; }
}