﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Users.Create;
using Sharecode.Backend.Utilities.KeyValue;

namespace Sharecode.Backend.Api.Controller;

[Route("[controller]")]
[ApiController]
public class KeyController : ControllerBase
{
    private readonly IMediator _mediator;

    public KeyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Result([FromBody] CreateUserCommand command)
    {
        await _mediator.Send(command);
        return CreatedAtRoute("user", command);
    }
}