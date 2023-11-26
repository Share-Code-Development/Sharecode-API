using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Gateway.Validate;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Api.Controller;

public class GatewayController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{
    [HttpPatch("{gatewayType}/{id}")]
    public async Task<IActionResult> ValidateGateway([FromRoute] int gatewayType, [FromRoute] Guid id, [FromBody] ValidateGatewayAppRequest gatewayAppRequest)
    {
        if (!Enum.IsDefined(typeof(GatewayRequestType), gatewayType))
        {
            return BadRequest($"Unknown gateway type is provided");
        }
        gatewayAppRequest.GatewayId = id;
        gatewayAppRequest.Type = (GatewayRequestType)gatewayType;
        var commandResponse = await mediator.Send(gatewayAppRequest);

        if (commandResponse.Response != HttpStatusCode.OK)
        {
            return StatusCode((int)commandResponse.Response, commandResponse);
        }

        return Ok(commandResponse);
    }
}