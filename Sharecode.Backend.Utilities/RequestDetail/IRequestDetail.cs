namespace Sharecode.Backend.Utilities.RequestDetail;

public interface IRequestDetail : IBaseRequestDetail
{
    public string? ConnectingAddress { get; protected set; }
    public string? OriginCountry { get; protected set; }
    public IReadOnlyList<string> XForwardedFor { get; protected set;}
}