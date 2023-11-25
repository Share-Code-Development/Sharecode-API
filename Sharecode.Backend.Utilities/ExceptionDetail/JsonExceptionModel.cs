namespace Sharecode.Backend.Utilities.JsonExceptions;

public class JsonExceptionModel
{
    public string ErrorDescription { get; init; }
    public long ErrorCode { get; init; } 
    public string NormalizedClassName { get; init; }
}