using MediatR;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Gateway;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Events;
using Sharecode.Backend.Domain.Events.Users;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.Email;

namespace Sharecode.Backend.Application.Events.User;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedDomainEvent>
{

    private readonly IEmailClient _emailClient;
    private readonly IGatewayRepository _gatewayRepository;
    private readonly IGatewayService _gatewayService;

    public UserCreatedEventHandler(IEmailClient emailClient, IGatewayRepository gatewayRepository, IGatewayService gatewayService)
    {
        _emailClient = emailClient;
        _gatewayRepository = gatewayRepository;
        _gatewayService = gatewayService;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await SendVerificationEmailAsync(notification, cancellationToken);

    }

    private async Task SendVerificationEmailAsync(UserCreatedDomainEvent notification, CancellationToken cancellationToken = default)
    {
        var requestAsync = await CreateGatewayRequestAsync(notification, cancellationToken);
        if(requestAsync == null)
            return;
        
        await _gatewayRepository.AddAsync(requestAsync, cancellationToken);
        
        var placeholders = new Dictionary<string, string> { { "VERIFICATION_URL", $"{requestAsync.Id}" }, {"USER_NAME", notification.FullName} };
        await _emailClient.SendTemplateMailAsync(
            EmailTemplateKeys.EmailValidation,
            new EmailTargets(notification.EmailAddress),
            placeholders
        );
    }

    private async Task<GatewayRequest?> CreateGatewayRequestAsync(UserCreatedDomainEvent notification, CancellationToken cancellationToken = default)
    {
        bool limitReached = await _gatewayService.IsLimitReachedAsync(notification.UserId, GatewayRequestType.VerifyUserAccount, cancellationToken);
        if (limitReached)
            return null;

        return GatewayRequest.CreateRequest(GatewayRequestType.VerifyUserAccount, notification.UserId);
    }
}