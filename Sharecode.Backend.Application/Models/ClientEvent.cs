namespace Sharecode.Backend.Application.Models;

public class ClientEvent
{
    public string EventType { get; set; }
    public object Event { get; set; }
}