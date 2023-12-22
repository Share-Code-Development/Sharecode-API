using MediatR;
using Microsoft.Extensions.Logging;
using Sharecode.Backend.Domain.Events.Users;
using Sharecode.Backend.Utilities.Email;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Application.Events.User;

public class UserVerifiedEventHandler(IEmailClient emailClient, ILogger<UserVerifiedEventHandler> logger)
    : INotificationHandler<UserVerifiedDomainEvent>
{
    public async Task Handle(UserVerifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        await emailClient.SendTemplateMailAsync(
            EmailTemplateKeys.WelcomeUser, 
            new EmailTargets().AddTarget(notification.EmailAddress, notification.FullName),
            new Dictionary<string, string>() {{EmailPlaceholderKeys.UserNameKey, notification.FullName}},
            new Dictionary<string, string>() {{EmailPlaceholderKeys.UserNameKey, notification.FullName}}
        );
        logger.LogInformation($"Welcome email has been sent to user {notification.EmailAddress}");
    }
}