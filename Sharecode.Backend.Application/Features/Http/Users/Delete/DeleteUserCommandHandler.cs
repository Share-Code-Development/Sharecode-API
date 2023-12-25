using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Data;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Application.Service;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Application.Features.Http.Users.Delete;

public class DeleteUserCommandHandler(IUserService service, IHttpClientContext clientContext, ILogger logger, IUnitOfWork unitOfWork) : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userIdentifier = await clientContext.GetUserIdentifierAsync() ?? Guid.Empty;
            bool isHardDelete = !request.SoftDelete ?? false;
            bool sameUser = request.UserId == userIdentifier;
        
            //If to do a hard delete or to delete another user, you need admin permissions
            bool requireElevatedPerms = isHardDelete || !sameUser;
            if (requireElevatedPerms && !await clientContext.HasPermissionAsync(Permissions.DeleteSnippetOthers, cancellationToken))
            {
                throw new NotEnoughPermissionException("Hard delete of user", showPermission: false,
                    Permissions.DeleteSnippetOthers);
            }
            logger.Information("An account deletion has been requested by {RequestUser} on Account Id {ToDelete}. Is the request to soft delete = {SoftDelete}", userIdentifier, request.UserId, request.SoftDelete);
            var success = await service.DeleteUser(request.UserId, userIdentifier, request.SoftDelete ?? false, cancellationToken);
            logger.Information("The requested account deletion has been processed on Account Id {ToDelete}. Is the request to soft delete = {SoftDelete}. The response is {Response}", userIdentifier, request.UserId, request.SoftDelete, success);
        
            return new DeleteUserResponse { Success = success };
        }
        finally
        {
            await unitOfWork.CommitAsync(cancellationToken);
        }
    }
}