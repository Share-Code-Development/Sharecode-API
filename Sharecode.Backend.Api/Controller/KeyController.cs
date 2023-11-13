using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Sharecode.Backend.Application.Users.Create;
using Sharecode.Backend.Domain.Dto;
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

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        return Ok(1);
    }

    [HttpPost]
    public async Task<IActionResult> Result([FromBody] CreateUserCommand command)
    {
        UserCreatedResponse response = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new {id = response.UserId}, response);
    }
}