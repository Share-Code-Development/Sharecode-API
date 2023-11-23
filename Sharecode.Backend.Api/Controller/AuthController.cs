using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Users.Create;
using Sharecode.Backend.Application.Features.Users.Login;

namespace Sharecode.Backend.Api.Controller;

public class AuthController(IDistributedCache cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{

    [HttpPost(template: "register/", Name = "Register User")]
    public async Task<ActionResult> Register([FromBody] CreateUserCommand command)
    {
        UserCreatedResponse response = await mediator.Send(command);
        return CreatedAtRoute("Get User", new { id = response.UserId }, response);
    }

    [HttpPost(template:"login/", Name = "Login user")]
    public async Task<ActionResult> Login([FromBody] LoginUserRequest request)
    {
        LoginUserResponse userResponse = await mediator.Send(request);
        return Ok(userResponse);
    }
}