using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;

namespace Sharecode.Backend.Api.Controller;

[EnableRateLimiting("fixed")]
public class HealthController(IDistributedCache cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{
    [HttpGet("_health")]
    public ActionResult Health()
    {
        return Ok();
    }
}