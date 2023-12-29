namespace Sharecode.Backend.Application.Models;

public class LiveEventConnectionRefused(string reasonProvided)
{
    public string ReasonProvided { get; set; } = reasonProvided;
}