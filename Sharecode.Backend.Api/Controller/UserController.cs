using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Http.Users.Delete;
using Sharecode.Backend.Application.Features.Http.Users.Get;
using Sharecode.Backend.Application.Features.Http.Users.GetMySnippets;
using Sharecode.Backend.Application.Features.Http.Users.Metadata.Delete;
using Sharecode.Backend.Application.Features.Http.Users.Metadata.List;
using Sharecode.Backend.Application.Features.Http.Users.Metadata.Upsert;
using Sharecode.Backend.Application.Features.Http.Users.TagSearch;
using Sharecode.Backend.Application.Features.Http.Users.Usage;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.Extensions;
using Sharecode.Backend.Utilities.RedisCache;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.Controller;


public class UserController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{
    /// <summary>
    /// Retrieves a user based on the provided ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <param name="includeSettings">Whether to include user settings in the response. Defaults to false.</param>
    /// <returns>An ActionResult containing a GetUserResponse object representing the retrieved user.</returns>
    [HttpGet("{id}", Name = "Get User")]
    [Authorize]
    public async Task<ActionResult<GetUserResponse>> GetUser([FromRoute] Guid id, [FromQuery] bool includeSettings = false)
    {
        FrameCacheKey(CacheModules.User,id.ToString(), GetQuery());
        var cachedResponse = await ScanAsync<GetUserResponse>(true);
        if (cachedResponse != null)
        {
            return Ok(cachedResponse);
        }
        
        var idQuery = new GetUserByIdQuery(id, includeSettings);
        GetUserResponse userResponse = await mediator.Send(idQuery, RequestCancellationToken);
        await StoreCacheAsync(userResponse,token: RequestCancellationToken);
        return Ok(userResponse);
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="emailAddress">The email address of the user.</param>
    /// <param name="includeSettings">Optional. Indicates whether to include user settings in the response. Default value is false.</param>
    /// <returns>An ActionResult of type GetUserResponse that represents the user information.</returns>
    [HttpGet("email/{emailAddress}", Name = "Get User By Email")]
    [Authorize]
    public async Task<ActionResult<GetUserResponse>> GetUser([FromRoute] string emailAddress, [FromQuery] bool includeSettings = false)
    {
        FrameCacheKey(CacheModules.User,emailAddress, GetQuery());
        var cachedResponse = await ScanAsync<GetUserResponse>(true);
        if (cachedResponse != null)
        {
            return Ok(cachedResponse);
        }
        
        var emailQuery = new GetUserByEmailQuery(emailAddress, includeSettings);
        GetUserResponse userResponse = await mediator.Send(emailQuery, RequestCancellationToken);
        await StoreCacheAsync(userResponse,token: RequestCancellationToken);
        return Ok(userResponse);
    }

    [HttpGet("{id}/snippet-usage", Name = "Get the snippet usage of the user")]
    [Authorize]
    public async Task<ActionResult<GetUserSnippetUsageResponse>> GetUsersSnippetUsage([FromRoute] Guid id)
    {
        FrameCacheKey(CacheModules.UserSnippetUsage, id.ToString());
        //Only user can invalidate it by creating new snippet. So update the current ttl if its accessed once. If the usage
        //is changed, Update/Delete/Create Snippet will remove this 
        var cachedResponse = await ScanAsync<GetUserSnippetUsageResponse>(true);
        if (cachedResponse != null)
        {
            return cachedResponse;
        }

        var request = new GetUserSnippetUsageQuery()
        {
            UserId = id
        };
        var response = await mediator.Send(request);
        await StoreCacheAsync(response, token: RequestCancellationToken);
        return response;
    }

    /// <summary>
    /// Gets a list of users to tag based on the given search command.
    /// </summary>
    /// <param name="command">The search command containing the criteria for finding users.</param>
    /// <returns>An ActionResult of type SearchUserForTagResponse, representing the list of users to tag.</returns>
    [HttpGet("tag-search", Name = "Get users to tag")]
    public async Task<ActionResult<SearchUserForTagResponse>> GetUsersToTag([FromQuery] SearchUsersForTagCommand command)
    {
        if (await AppRequestContext.HasPermissionAsync(Permissions.ViewUserOtherAdmin))
        {
            FrameCacheKey(CacheModules.User, "search", "admin" ,GetQuery());
        }
        else
        {
            FrameCacheKey(CacheModules.User, "search", GetQuery());
        }
            
        var response = await ScanAsync<SearchUserForTagResponse>();
        if (response != null)
        {
            return Ok(response);
        }

        var tagResponse = await mediator.Send(command);
        await StoreCacheAsync(tagResponse, TimeSpan.FromMinutes(5), token: RequestCancellationToken);
        return Ok(tagResponse);
    }

    /// <summary>
    /// Retrieves the snippets for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="query">The query parameters for the snippets.</param>
    /// <returns>The snippets of the user.</returns>
    /// <exception cref="NoAccessException">Thrown if the requesting user does not have access to the snippets.</exception>
    [HttpGet("{userId}/snippets")]
    [Authorize]
    public async Task<ActionResult<GetMySnippetsResponse>> GetMySnippets([FromRoute] Guid userId,
        [FromQuery] GetMySnippetsQuery query)
    {
        var userInIdentity = await AppRequestContext.GetUserIdentifierAsync() ?? Guid.Empty;
        //If the requesting user is not the user who logged in then don't return
        if(userInIdentity != userId)
            throw new NoAccessException(userInIdentity, $"recent-snippets of {userId.ToString()}", typeof(Domain.Entity.Snippet.Snippet));

        if (query.RecentSnippets)
            FrameCacheKey(CacheModules.UserSnippet, userId.ToString(), "recent");
        else
            FrameCacheKey(CacheModules.UserSnippet, userId.ToString(), GetQuery());
        var cacheResponse = await ScanAsync<GetMySnippetsResponse>();
        if (cacheResponse != null)
            return Ok(cacheResponse);

        var response = await mediator.Send(query);
        await StoreCacheAsync(response, token: RequestCancellationToken);
        return Ok(response);
    }

    [HttpDelete("{userId}")]
    [Authorize]
    public async Task<ActionResult> DeleteUserAccount([FromRoute]Guid userId, [FromQuery] DeleteUserCommand deleteUserCommand)
    {
        deleteUserCommand.UserId = userId;
        var deleteUserResponse = await mediator.Send(deleteUserCommand, RequestCancellationToken);

        if (deleteUserResponse.Success)
        {
            await ClearCacheAsync();
            return NoContent();
        }
        
        return NotFound();
    }

    #region Metadata

    [HttpGet("{userId}/metadata")]
    public async Task<ActionResult> ListMetadata([FromRoute] Guid userId, [FromQuery] string keys)
    {
        FrameCacheKey(CacheModules.UserMetadata, userId.ToString(), GetQuery());
        var cacheResponse = await ScanAsync<ListUserMetadataResponse>();
        if (cacheResponse != null)
        {
            return Ok(cacheResponse);
        }

        var query = new ListUserMetadataQuery()
        {
            UserId = userId
        };
        var keySet = keys.Split(",").ToHashSet();
        query.Queries.AddRange(keySet);
        var response = await mediator.Send(query);
        if (response == null)
        {
            return NoContent();
        }

        return Ok(response);
    }
    
    [HttpPost("{userId}/metadata")]
    public async Task<ActionResult> UpsertMetadata([FromRoute] Guid userId, [FromBody] UpsertUserMetadataCommand command)
    {
        command.UserId = userId;
        var res = await mediator.Send(command);
        await ClearCacheAsync();
        return Ok(res);
    }
    
    [HttpDelete("{userId}/metadata")]
    public async Task<ActionResult> DeleteMetadata([FromRoute] Guid userId, [FromBody] DeleteUserMetadataCommand command)
    {
        command.UserId = userId;
        await mediator.Send(command);
        
        await ClearCacheAsync();
        return NoContent();
    }

    #endregion
    
}