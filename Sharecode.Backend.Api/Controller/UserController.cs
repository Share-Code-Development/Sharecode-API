using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Users.Get;
using Sharecode.Backend.Application.Features.Users.TagSearch;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Api.Controller;


public class UserController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{

    [HttpGet("{id}", Name = "Get User")]
    [Authorize]
    public async Task<ActionResult> GetUser(Guid id, [FromQuery] bool includeSettings = false)
    {
        FrameCacheKey("user",id.ToString(), GetQuery());
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
    
    
    [HttpGet("email/{emailAddress}", Name = "Get User By Email")]
    [Authorize]
    public async Task<ActionResult> GetUser(string emailAddress, [FromQuery] bool includeSettings = false)
    {
        FrameCacheKey("user",emailAddress, GetQuery());
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

    [HttpGet("tag-search", Name = "Get users to tag")]
    public async Task<ActionResult> GetUsersToTag([FromQuery] SearchUsersForTagCommand command)
    {
        //TODO Add admin role to the cache
        FrameCacheKey("user", "search", GetQuery());
        var response = await ScanAsync<SearchUserForTagResponse>();
        if (response != null)
        {
            return Ok(response);
        }

        var tagResponse = await mediator.Send(command);
        await StoreCacheAsync(tagResponse, TimeSpan.FromMinutes(5), token: RequestCancellationToken);
        return Ok(tagResponse);
    }

    [HttpGet("{userId}/snippets")]
    public async Task<ActionResult> GetMySnippets([FromRoute] Guid userId)
    {
        FrameCacheKey("user-snippets", userId.ToString(), GetQuery());

        return Ok();
    }
    
}