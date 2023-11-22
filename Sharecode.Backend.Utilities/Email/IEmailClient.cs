namespace Sharecode.Backend.Utilities.Email;

public interface IEmailClient
{
    Task SendTemplateMailAsync(EmailTemplateKey templateKey, EmailTargets targets, Dictionary<string, string>? placeholders = null, Dictionary<string, string>? subjectPlaceholders = null);
}