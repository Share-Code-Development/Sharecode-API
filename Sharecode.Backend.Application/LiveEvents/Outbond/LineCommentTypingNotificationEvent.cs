namespace Sharecode.Backend.Application.Event.Outbond;

public class LineCommentTypingNotificationEvent
{
    public Guid? UserIdentifier { get; set; }
    public string UserName { get; set; }
    public int LineNumber { get; set; }
    public bool Action { get; set; }
}