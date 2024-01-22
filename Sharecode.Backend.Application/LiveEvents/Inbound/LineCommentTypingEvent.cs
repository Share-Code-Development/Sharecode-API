namespace Sharecode.Backend.Application.LiveEvents.Inbound;

public class LineCommentTypingEvent
{
    public Guid SnippetId { get; set; }
    public int LineNumber { get; set; }
    public bool Action { get; set; }
}