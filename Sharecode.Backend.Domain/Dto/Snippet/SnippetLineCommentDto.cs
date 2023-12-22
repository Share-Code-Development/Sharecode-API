namespace Sharecode.Backend.Domain.Dto.Snippet;

public class SnippetLineCommentDto
{
    public string Id { get; set; } 
    public string FirstName { get; set; } 
    public string MiddleName { get; set; } 
    public string LastName { get; set; } 
    public string EmailAddress { get; set; } 
    public int LineNumber { get; set; } 
    public string Text { get; set; } 
    public DateTime CreatedAt { get; set; } 
    public DateTime ModifiedAt { get; set; }
}