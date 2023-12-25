using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Http.Gateway.Validate;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Api.Controller;

public class GatewayController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{
    /// <summary>
    /// Validates the gateway based on the provided information.
    /// </summary>
    /// <param name="gatewayType">The type of the gateway.</param>
    /// <param name="id">The ID of the gateway.</param>
    /// <param name="gatewayAppRequest">The request object containing the gateway information.</param>
    /// <returns>The validation response for the gateway.</returns>
    [HttpPatch("{gatewayType}/{id}")]
    public async Task<ActionResult<ValidateGatewayCommandResponse>> ValidateGateway([FromRoute] int gatewayType,
        [FromRoute] Guid id, [FromBody] ValidateGatewayAppRequest gatewayAppRequest)
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