using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Application.Features.Http.Users.Metadata.Upsert;

public class UpsertMetadataCommandHandler(ILogger logger, IHttpClientContext context, IUserService userService, IUnitOfWork unitOfWork) : IRequestHandler<UpsertUserMetadataCommand, UpsertUserMetadataResponse>
{
    public async Task<UpsertUserMetadataResponse> Handle(UpsertUserMetadataCommand request, CancellationToken cancellationToken)
    {
        var requestingUserIdRaw = await context.GetUserIdentifierAsync();
        if(!requestingUserIdRaw.HasValue)
            throw new NoAccessException($"{request.UserId.ToString()}/metadata", Guid.Empty, typeof(User));
        
        var requestingUserId = requestingUserIdRaw.Value;
        if (requestingUserId != request.UserId)
        {
            if (!await context.HasPermissionAsync(Permissions.UpdateUserOtherAdmin, cancellationToken))
            {
                throw new NotEnoughPermissionException("Update user metadata");
            }
        }

        if (!request.MetaDictionary.Any())
        {
            return UpsertUserMetadataResponse.Empty;
        }

        var oldValues = await userService.UpdateExternalMetadataAsync(request.UserId, request.MetaDictionary, cancellationToken);
        context.AddCacheKeyToInvalidate(CacheModules.UserMetadata, request.UserId.ToString());
        await unitOfWork.CommitAsync(cancellationToken);
        return UpsertUserMetadataResponse.From(oldValues);
    }
}