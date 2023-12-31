using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Utilities;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Application.Features.Http.Users.Logout;

public class LogoutCommandHandler(ILogger logger, IHttpClientContext context, IJwtClient jwtClient, Namespace keyValueNamespace) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var accessTokenEncryptionKey = keyValueNamespace.Of(KeyVaultConstants.JwtAccessTokenEncryptionKey)?.Value ?? string.Empty;
        var accessTokenSecretKey = keyValueNamespace.Of(KeyVaultConstants.JwtSecretKey)?.Value ?? string.Empty;
        
        if(string.IsNullOrEmpty(request.AccessToken))
            return;
        
        var claims = jwtClient.ValidateToken(request.AccessToken, accessTokenSecretKey, accessTokenEncryptionKey);
        
        
        return;
    }
}