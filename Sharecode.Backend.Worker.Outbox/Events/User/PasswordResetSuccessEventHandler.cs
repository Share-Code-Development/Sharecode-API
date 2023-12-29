using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharecode.Backend.Domain.Events.Users;

namespace Sharecode.Backend.Application.Events.User;

public class PasswordResetSuccessEventHandler : INotificationHandler<PasswordResetSuccessDomainEvent>
{
    public async Task Handle(PasswordResetSuccessDomainEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine("Updated");
        return;
    }
}