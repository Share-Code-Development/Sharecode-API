using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Refresh.Get;
using Sharecode.Backend.Application.Features.Users.Create;
using Sharecode.Backend.Application.Features.Users.Login;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Api.Controller;

public class AuthController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{

    private const string ExposeHeadersString = $"Authorization, Expires";

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

    [HttpGet(template: "refresh/", Name = "Refresh the access token")]
    public async Task<ActionResult> Refresh()
    {
        if (!TryGetHeader("XCS-Refresh-Token", out string? refreshToken))
        {
            return Unauthorized($"No refresh token passed in the header XCS-Refresh-Token");
        }

        var command = new GetRefreshTokenCommand(refreshToken!);
        var response = await mediator.Send(command);
        Response.Headers.AccessControlExposeHeaders = ExposeHeadersString;
        Response.Headers.Authorization = response.RefreshToken;
        Response.Headers.Expires = response.Expiry.Ticks.ToString();
        return Ok();
    }
}