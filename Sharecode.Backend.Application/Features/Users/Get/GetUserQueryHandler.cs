using MediatR;
using Serilog;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Domain.Repositories;

namespace Sharecode.Backend.Application.Features.Users.Get;

public class GetUserQueryByIdHandler(IUserRepository repository, IHttpClientContext context, ILogger logger) : IRequestHandler<GetUserByIdQuery, GetUserResponse>
{
    public async Task<GetUserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var requesterId = await context.GetUserIdentifierAsync();
        if (request.UserId != requesterId)
        {
            request = new GetUserByIdQuery(request.UserId, false);
            logger.Information($"Disabled include settings since the request is coming from {requesterId} for {request.UserId}");
        }
        
        if (context.IsApiRequest)
        {
            //TODO Handle Later
        }
        else
        {
            if (request.UserId != requesterId)
            {
                var hasPermission = await context.HasPermissionAsync(Permissions.AccessProfileOthers, cancellationToken);
                if (!hasPermission)
                    throw new NoAccessException(requesterId ?? Guid.Empty, request.UserId, typeof(User));
            }
        }
        var user = await repository.GetUserByIdIncludingAccountSettings(request.UserId, request.IncludeSettings, token: cancellationToken);
        if (user == null)
            throw new EntityNotFoundException(typeof(User), request.UserId);

        Guid? requestingUser = await context.GetUserIdentifierAsync();
        if (requestingUser != user.Id && user.Visibility == AccountVisibility.Private)
            throw new ProfileIsPrivateException();

        return GetUserResponse.From(user);
    }
}

public class GetUserQueryByEmailHandler(IUserRepository repository, IHttpClientContext context, ILogger logger) : IRequestHandler<GetUserByEmailQuery, GetUserResponse>
{
    public async Task<GetUserResponse> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        //Check whether the requesting user can access the user details or not
        var requesterEmailAddress = context.EmailAddress;
        if (request.EmailAddress != requesterEmailAddress)
        {
            request = new GetUserByEmailQuery(request.EmailAddress, false);
            logger.Information($"Disabled include settings since the request is coming from {requesterEmailAddress} for {request.EmailAddress}");
        }
        if (context.IsApiRequest)
        {
            //TODO Handle Later
        }
        else
        {
            if (request.EmailAddress != requesterEmailAddress)
            {
                var hasPermission = await context.HasPermissionAsync(Permissions.AccessProfileOthers, cancellationToken);
                if (!hasPermission)
                    throw new NoAccessException(requesterEmailAddress ?? string.Empty, request.EmailAddress, typeof(User));
            }
        }
        
        var user = await repository.GetUserByEmailIncludingAccountSettings(request.EmailAddress, request.IncludeSettings, token: cancellationToken);
        if (user == null)
            throw new EntityNotFoundException(typeof(User), request.EmailAddress);

        Guid? requestingUser = await context.GetUserIdentifierAsync();
        if (requestingUser != user.Id && user.Visibility == AccountVisibility.Private)
            throw new ProfileIsPrivateException();

        return GetUserResponse.From(user);    
    }
    
}