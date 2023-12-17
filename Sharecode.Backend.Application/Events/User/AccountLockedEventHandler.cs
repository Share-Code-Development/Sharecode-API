using System.Globalization;
using MediatR;
using Microsoft.Extensions.Options;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Gateway;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Events.Users;
using Sharecode.Backend.Domain.Extensions;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.Email;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Application.Events.User;

public class AccountLockedEventHandler(IRefreshTokenService refreshTokenService, IUserService userService, IEmailClient emailClient , IAppCacheClient cacheClient, IGatewayService gatewayService, IOptions<FrontendConfiguration> configuration) : INotificationHandler<AccountLockedDomainEvent>
{
    private readonly FrontendConfiguration _frontendConfiguration = configuration.Value;
    public async Task Handle(AccountLockedDomainEvent notification, CancellationToken cancellationToken)
    {
        
        Console.WriteLine(_frontendConfiguration.Base);
        await refreshTokenService.InvalidateAllOfUserAsync(notification.UserId, cancellationToken);
        var matchingCacheKeys = new List<string>
        {
            $"user-{notification.UserId}-*", //Delete all the data from cache with user id
            $"user-{notification.EmailAddress}-*" //Delete all the data from cache with email address
        };
        await cacheClient.DeleteMatchingKeysAsync(matchingCacheKeys, cancellationToken);
        var gatewayRequest = await gatewayService.CreateGatewayRequestAsync(notification.UserId, GatewayRequestType.ForgotPassword, true,
            DateTime.UtcNow.AddDays(5), cancellationToken);

        if (gatewayRequest == null)
        {
            return;
        }

        var baseUrl = configuration.Value.Base;
        var gatewayUrl = GatewayRequestType.ForgotPassword.CreateGatewayUrl(baseUrl, gatewayRequest.Id);
        await emailClient.SendTemplateMailAsync(EmailTemplateKeys.AccountLocked,
            new EmailTargets(notification.EmailAddress),
            new Dictionary<string, string>()
            {
                { EmailPlaceholderKeys.GatewayUrlKey, gatewayUrl },
                { EmailPlaceholderKeys.UserNameKey, notification.FullName },
                { EmailPlaceholderKeys.AccountLockedLastAttemptKey, $"{notification.LastOccurence.ToString(CultureInfo.InvariantCulture)} UTC" },
                { EmailPlaceholderKeys.AccountLockedCountry, notification.RequestDetail.OriginCountry ?? string.Empty },
                { EmailPlaceholderKeys.AccountLockedIpAddress, notification.RequestDetail.ConnectingAddress ?? string.Empty }
            },
            null
        );
    }
}