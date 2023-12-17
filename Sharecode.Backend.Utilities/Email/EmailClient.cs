
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Utilities.Email;

public class EmailClient(IOptions<LocalDirectoryConfiguration> directoryConfiguration, Namespace keyValueNameSpace)
    : IEmailClient
{

    private readonly LocalDirectoryConfiguration _directoryConfiguration = directoryConfiguration.Value;
    private string? _host, _password, _username, _from, _fromName;
    private int? _port;
    private readonly ILogger _logger = Log.ForContext(typeof(EmailClient));

    public async Task SendTemplateMailAsync(EmailTemplateKey templateKey, EmailTargets targets, Dictionary<string, string>? placeholders, Dictionary<string, string>? subjectPlaceholders = null)
    {
        var htmlTemplate = await templateKey.GetOrFetchAsync(_directoryConfiguration.EmailTemplates);
        if (string.IsNullOrEmpty(htmlTemplate))
        {
            _logger.Fatal($"Failed to get the template for {templateKey.Key}. Aborting mail sending!");
            return;
        }
        EmailTemplate emailTemplate = new EmailTemplate(htmlTemplate, placeholders);
        emailTemplate.ParseAsync();

        if(string.IsNullOrEmpty(_host))
            _host = keyValueNameSpace.Of(KeyVaultConstants.SmtpHost)?.Value!;
        if(string.IsNullOrEmpty(_username))
            _username = keyValueNameSpace.Of(KeyVaultConstants.SmtpUserName)?.Value!;
        if(string.IsNullOrEmpty(_password))
            _password = keyValueNameSpace.Of(KeyVaultConstants.SmtpPassword)?.Value!;
        if(string.IsNullOrEmpty(_from))
            _from = keyValueNameSpace.Of(KeyVaultConstants.SmtpFrom)?.Value!;

        if(string.IsNullOrEmpty(_fromName))
            _fromName = keyValueNameSpace.Of(KeyVaultConstants.SmtpFromName)?.Value!;

        if (!_port.HasValue)
        {
            var portRaw = keyValueNameSpace.Of(KeyVaultConstants.SmtpPort)?.Value;

            if (string.IsNullOrEmpty(portRaw))
                throw new ArgumentNullException(nameof(_port));
        
            if (!int.TryParse(portRaw, out var port))
            {
                if (string.IsNullOrEmpty(portRaw))
                    throw new ArgumentNullException(nameof(port));
            }
            _port = port;
        }

        var subject = templateKey.Subject;
        if (subjectPlaceholders != null)
        {
            foreach (var placeholder in subjectPlaceholders)
            {
                subject = subject.Replace("{("+placeholder.Key+")}", placeholder.Value);
            }
        }



        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = emailTemplate.TemplateHtml;
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Sharecode", _from));
        mailMessage.To.Add(new MailboxAddress("Alen Geo Alex", targets.Target));
        mailMessage.Subject = subject;
        mailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, _port!.Value, SecureSocketOptions.Auto);
        await client.AuthenticateAsync(_username, _password);
        await client.SendAsync(mailMessage);
    }
}
