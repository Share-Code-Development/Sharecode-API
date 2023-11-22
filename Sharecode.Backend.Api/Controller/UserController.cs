using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;

namespace Sharecode.Backend.Api.Controller;

public class UserController(IDistributedCache cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{

    [HttpGet("{id}", Name = "Get User")]
    public async Task<ActionResult> GetUser(Guid id)
    {
        return Ok();
    }
    
}