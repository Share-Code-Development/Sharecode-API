using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Users.Get;

namespace Sharecode.Backend.Api.Controller;

[Authorize]
public class UserController(IDistributedCache cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{

    [HttpGet("{id}", Name = "Get User")]
    [Authorize]
    public async Task<ActionResult> GetUser(Guid id)
    {
        var idQuery = new GetUserByIdQuery(id);
        GetUserResponse userResponse = await mediator.Send(idQuery);
        return Ok(userResponse);
    }
    
    
    [HttpGet("email/{emailAddress}", Name = "Get User By Email")]
    [Authorize]
    public async Task<ActionResult> GetUser(string emailAddress)
    {
        var emailQuery = new GetUserByEmailQuery(emailAddress);
        GetUserResponse userResponse = await mediator.Send(emailQuery);
        return Ok(userResponse);
    }
    
}