namespace Sharecode.Backend.Utilities.JsonExceptions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ExceptionDetail(int errorCode, string errorDescription) : Attribute
{
    public long ErrorCode { get; set; } = errorCode;
    public string ErrorDescription { get; set; } = errorDescription;
}