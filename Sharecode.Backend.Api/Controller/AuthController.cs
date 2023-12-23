using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Application.Features.Refresh.Get;
using Sharecode.Backend.Application.Features.Users.Create;
using Sharecode.Backend.Application.Features.Users.ForgotPassword;
using Sharecode.Backend.Application.Features.Users.Login;
using Sharecode.Backend.Utilities.RedisCache;

namespace Sharecode.Backend.Api.Controller;

public class AuthController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{

    private const string ExposeHeadersString = $"Authorization, Expires, RefreshToken";

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="command">The create user command.</param>
    /// <returns>The result of the registration process.</returns>
    [HttpPost(template: "register/", Name = "Register User")]
    public async Task<ActionResult<UserCreatedResponse>> Register([FromBody] CreateUserCommand command)
    {
        UserCreatedResponse response = await mediator.Send(command);
        return CreatedAtRoute("Get User", new { id = response.UserId }, response);
    }

    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="request">The user's login request.</param>
    /// <returns>The login response.</returns>
    [HttpPost(template: "login/", Name = "Login user")]
    public async Task<ActionResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request)
    {
        LoginUserResponse userResponse = await mediator.Send(request);
        return Ok(userResponse);
    }

    /// <summary>
    /// Refreshes the access token.
    /// </summary>
    /// <returns>An <see cref="ActionResult"/> representing the result of the operation.</returns>
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
        Response.Headers.Authorization = response.AccessToken;
        Response.Headers.Append("RefreshToken", response.RefreshToken);
        Response.Headers.Expires = response.RefreshTokenExpiry.Ticks.ToString();
        return Ok();
    }

    /// <summary>
    /// Sends a request to reset the forgotten password for the specified user.
    /// </summary>
    /// <param name="command">The ForgotPasswordCommand object containing the necessary information for resetting the password.</param>
    /// <returns>Returns an ActionResult<bool> indicating whether the password reset request was successful.</returns>
    [HttpPost("forgot-password/", Name = "Request a password forgot")]
    public async Task<ActionResult<bool>> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        var send = await mediator.Send(command);
        if (send)
            return Ok();
        
        return BadRequest();
    }
}