namespace Sharecode.Backend.Application.Features.Live.Snippet;

public class JoinedSnippetResponse
{
    public Guid SnippetId { get; init; }
    public Guid? JoinedUserId { get; init; }
    public string JoinedUserName { get; init; }
    public List<ActiveSnippetUsersDto> ActiveUsers { get; } = [];
}

public sealed record ActiveSnippetUsersDto
{
    public Guid? Id { get; init; }
    public string ConnectionId { get; init; }
    public string FullName { get; init; }
    public string? ProfilePicture { get; init; }
}