namespace Sharecode.Backend.Domain.Dto;

public class MentionDto
{
    public Guid UserId { get; set; }
    public string EmailAddress { get; set; }
    public string? ProfilePicture { get; set; }
    public string FullName { get; set; }
}