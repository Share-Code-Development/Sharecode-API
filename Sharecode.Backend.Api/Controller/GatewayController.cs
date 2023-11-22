using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Gateway.Validate;

namespace Sharecode.Backend.Api.Controller;

public class GatewayController(IDistributedCache cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{
    [HttpPatch("{id}")]
    public async Task<IActionResult> ValidateGateway(Guid id)
    {
        ValidateGatewayCommand gatewayCommand = new ValidateGatewayCommand(id);
        var commandResponse = await mediator.Send(gatewayCommand);

        if (commandResponse.Response != HttpStatusCode.OK)
        {
            return StatusCode((int)commandResponse.Response, commandResponse);
        }

        return Ok(commandResponse);
    }
}