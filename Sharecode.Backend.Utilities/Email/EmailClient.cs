
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Utilities.Email;

public class EmailClient : IEmailClient
{

    private readonly LocalDirectoryConfiguration _directoryConfiguration;
    private readonly Namespace _keyValueNameSpace;
    private string _host, _password, _username, _from, _fromName;
    private int? _port;

    public EmailClient(IOptions<LocalDirectoryConfiguration> directoryConfiguration, Namespace keyValueNameSpace)
    {
        _keyValueNameSpace = keyValueNameSpace;
        _directoryConfiguration = directoryConfiguration.Value;
    }
    
    public async Task SendTemplateMailAsync(EmailTemplateKey templateKey, EmailDeliveryDetail deliveryDetail, Dictionary<string, string>? placeholders)
    {
        var htmlTemplate = await templateKey.GetOrFetchAsync(_directoryConfiguration.EmailTemplates);
        EmailTemplate emailTemplate = new EmailTemplate(htmlTemplate, placeholders);
        emailTemplate.ParseAsync();

        if(string.IsNullOrEmpty(_host))
            _host = _keyValueNameSpace.Of(KeyVaultConstants.SmtpHost)?.Value!;
        if(string.IsNullOrEmpty(_username))
            _username = _keyValueNameSpace.Of(KeyVaultConstants.SmtpUserName)?.Value!;
        if(string.IsNullOrEmpty(_password))
            _password = _keyValueNameSpace.Of(KeyVaultConstants.SmtpPassword)?.Value!;
        if(string.IsNullOrEmpty(_from))
            _from = _keyValueNameSpace.Of(KeyVaultConstants.SmtpFrom)?.Value!;

        if(string.IsNullOrEmpty(_fromName))
            _fromName = _keyValueNameSpace.Of(KeyVaultConstants.SmtpFromName)?.Value!;

        if (!_port.HasValue)
        {
            var portRaw = _keyValueNameSpace.Of(KeyVaultConstants.SmtpPort)?.Value;

            if (string.IsNullOrEmpty(portRaw))
                throw new ArgumentNullException(nameof(_port));
        
            if (!int.TryParse(portRaw, out var port))
            {
                if (string.IsNullOrEmpty(portRaw))
                    throw new ArgumentNullException(nameof(port));
            }
            _port = port;
        }


        var bodyBuilder = new BodyBuilder();
        bodyBuilder.HtmlBody = emailTemplate.TemplateHtml;
        var mailMessage = new MimeMessage();
        mailMessage.From.Add(new MailboxAddress("Sharecode", _from));
        mailMessage.To.Add(new MailboxAddress("Alen Geo Alex", deliveryDetail.Target));
        mailMessage.Subject = templateKey.Subject;
        mailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_host, _port!.Value, SecureSocketOptions.Auto);
        await client.AuthenticateAsync(_username, _password);
        string async = await client.SendAsync(mailMessage);
    }
}
