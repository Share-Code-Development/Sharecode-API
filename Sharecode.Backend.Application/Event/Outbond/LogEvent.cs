namespace Sharecode.Backend.Application.Event;

public class LogEvent
{

    public static LogEvent Information(string message)
    {
        return new LogEvent("Info", message);
    }
    
    public static LogEvent Error(string message)
    {
        return new LogEvent("Error", message);
    }
    
    public static LogEvent Warning(string message)
    {
        return new LogEvent("Warning", message);
    }
    public string Level { get; init; }
    public string Message { get; set; }

    private LogEvent(string level, string message)
    {
        Level = level;
        Message = message;
    }
}