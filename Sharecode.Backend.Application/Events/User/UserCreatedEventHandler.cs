using MediatR;
using Sharecode.Backend.Domain.Events;

namespace Sharecode.Backend.Application.Events.User;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("User has been created");
        throw new Exception("Test Exception");
    }
}