namespace Sharecode.Backend.Utilities.RequestDetail;

public interface IBaseRequestDetail
{
    bool IsLocalEnvironment => !IsCloudflareEnvironment;
    bool IsCloudflareEnvironment { get; protected set; }
    
    string? UserAgent { get; protected set; }
}