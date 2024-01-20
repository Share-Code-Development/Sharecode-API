using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;

namespace Sharecode.Backend.Application.Features.Http.Users.Usage;

public class GetUserSnippetUsageQueryHandler(IHttpClientContext context, ISnippetService snippetService, ILogger logger) : IRequestHandler<GetUserSnippetUsageQuery, GetUserSnippetUsageResponse>
{
    public async Task<GetUserSnippetUsageResponse> Handle(GetUserSnippetUsageQuery request, CancellationToken cancellationToken)
    {
        var requesterId = await context.GetUserIdentifierAsync();
        
        if (request.UserId != requesterId)
        {
            var hasPermission = await context.HasPermissionAnyAsync(cancellationToken, Permissions.ViewUserOtherAdmin, Permissions.ViewUserOtherMinimal);
            if (!hasPermission)
                throw new NoAccessException(requesterId ?? Guid.Empty, request.UserId, "Snippet Usage");
        }

        var usageAsync = await snippetService.GetUserSnippetUsageAsync(request.UserId, cancellationToken);
        return new GetUserSnippetUsageResponse()
        {
            SizeLong = usageAsync
        };
    }
}