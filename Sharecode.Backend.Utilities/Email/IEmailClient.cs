namespace Sharecode.Backend.Utilities.Email;

public interface IEmailClient
{
    Task SendTemplateMailAsync(EmailTemplateKey templateKey, EmailDeliveryDetail deliveryDetail, Dictionary<string, string>? placeholders = null);
}