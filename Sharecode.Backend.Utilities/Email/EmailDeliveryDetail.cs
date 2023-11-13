namespace Sharecode.Backend.Utilities.Email;

public class EmailDeliveryDetail
{
    public readonly string Target;
    public EmailDeliveryDetail(string target)
    {
        Target = target;
    }
}