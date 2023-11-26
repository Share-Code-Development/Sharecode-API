using MediatR;
using Microsoft.Extensions.Options;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Gateway;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Events.Users;
using Sharecode.Backend.Domain.Extensions;
using Sharecode.Backend.Domain.Repositories;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.Email;

namespace Sharecode.Backend.Application.Events.User;

public class RequestForgotPasswordEventHandler(IEmailClient emailClient, IGatewayRepository gatewayRepository,
    IGatewayService gatewayService, IOptions<FrontendConfiguration> configuration) : INotificationHandler<RequestPasswordResetDomainEvent>
{
    public async Task Handle(RequestPasswordResetDomainEvent notification, CancellationToken cancellationToken)
    {
        var requestAsync = await CreateGatewayRequestAsync(notification, cancellationToken);
        if(requestAsync == null)
            return;
        
        await gatewayRepository.AddAsync(requestAsync, cancellationToken);

        var gatewayUrl = GatewayRequestType.ForgotPassword.CreateGatewayUrl(configuration.Value.Base, requestAsync.Id);

        var placeholders = new Dictionary<string, string> { { "RESET_PASSWORD_URL", $"{gatewayUrl}" }, {"USER_NAME", notification.FullName} };
        var subjectPlaceholders = new Dictionary<string, string>() { { "USER", notification.FullName } };
        await emailClient.SendTemplateMailAsync(
            EmailTemplateKeys.ResetPassword,
            new EmailTargets(notification.EmailAddress),
            placeholders,
            subjectPlaceholders
        );
    }
    
    private async Task<GatewayRequest?> CreateGatewayRequestAsync(RequestPasswordResetDomainEvent notification, CancellationToken cancellationToken = default)
    {
        var requestType = GatewayRequestType.ForgotPassword;
        bool limitReached = await gatewayService.IsLimitReachedAsync(notification.UserId, requestType , cancellationToken);
        if (limitReached)
            return null;

        return GatewayRequest.CreateRequest(requestType, notification.UserId);
    }
}