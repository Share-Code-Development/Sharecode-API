using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Http.Users.Metadata.List;

public class ListUserMetadataQueryHandler(ILogger logger, IHttpClientContext context, IUserRepository userRepository) : IRequestHandler<ListUserMetadataQuery, ListUserMetadataResponse?>
{
    public async Task<ListUserMetadataResponse?> Handle(ListUserMetadataQuery request, CancellationToken cancellationToken)
    {
        var requestingUserIdRaw = await context.GetUserIdentifierAsync();
        if(!requestingUserIdRaw.HasValue)
            throw new NoAccessException($"{request.UserId.ToString()}/{string.Join(',', request.Queries)}", Guid.Empty, typeof(User));

        var requestingUserId = requestingUserIdRaw.Value;
        if (requestingUserId != request.UserId)
        {
            if (!await context.HasPermissionAsync(Permissions.ViewUserOtherAdmin, cancellationToken))
            {
                throw new NotEnoughPermissionException("Access user metadata");
            }
        }

        var metadata = await userRepository.GetMetadataAsync(request.UserId, cancellationToken);
        return ListUserMetadataResponse.From(metadata, request.Queries);
    }
}