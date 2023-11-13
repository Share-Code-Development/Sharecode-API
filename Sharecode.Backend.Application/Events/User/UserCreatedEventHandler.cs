using MediatR;
using Sharecode.Backend.Domain.Events;
using Sharecode.Backend.Utilities.Email;

namespace Sharecode.Backend.Application.Events.User;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedDomainEvent>
{

    private readonly IEmailClient _emailClient;

    public UserCreatedEventHandler(IEmailClient emailClient)
    {
        _emailClient = emailClient;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("User has been created");
        var placeholders = new Dictionary<string, string> { { "VERIFICATION_URL", "Sample" }, {"USER_NAME", notification.FullName} };
        await _emailClient.SendTemplateMailAsync(
            EmailTemplateKeys.EmailValidation,
            new EmailDeliveryDetail(notification.EmailAddress),
            placeholders
        );
    }
}