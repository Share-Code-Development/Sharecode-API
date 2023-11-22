using MediatR;
using Microsoft.Extensions.Logging;
using Sharecode.Backend.Domain.Events.Users;
using Sharecode.Backend.Utilities.Email;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Application.Events.User;

public class UserVerifiedEventHandler: INotificationHandler<UserVerifiedDomainEvent>
{

    private readonly IEmailClient _emailClient;
    private readonly ILogger<UserVerifiedEventHandler> _logger;

    public UserVerifiedEventHandler(IEmailClient emailClient, ILogger<UserVerifiedEventHandler> logger)
    {
        _emailClient = emailClient;
        _logger = logger;
    }

    public async Task Handle(UserVerifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        await _emailClient.SendTemplateMailAsync(
            EmailTemplateKeys.WelcomeUser, 
            new EmailTargets(notification.EmailAddress),
            new Dictionary<string, string>() {{"USER", notification.FullName}},
            new Dictionary<string, string>() {{"WELCOME_USER", notification.FullName}}
        );
        _logger.LogInformation($"Welcome email has been sent to user {notification.EmailAddress}");
    }
}