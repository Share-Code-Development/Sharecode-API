using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Distributed;
using Sharecode.Backend.Application.Client;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Infrastructure;
using Sharecode.Backend.Utilities.Configuration;
using Sharecode.Backend.Utilities.ExceptionDetail;
using Sharecode.Backend.Utilities.JsonExceptions;
using Sharecode.Backend.Utilities.RedisCache;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.Controller;

[EnableRateLimiting("fixed")]
public class CommonController(IAppCacheClient cache, IHttpClientContext requestContext, ILogger logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{

    private static string _exceptionStrings = string.Empty;

    /// <summary>
    /// This method is used to check the health of the application.
    /// </summary>
    /// <returns>Returns an ActionResult with the request details if the application is healthy.</returns>
    [HttpGet]
    public ActionResult Health()
    {
        return Ok(requestContext.RequestDetail);
    }

    /// <summary>
    /// Retrieves the JSON representation of all registered exceptions in the application and writes it to the response.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpGet("exceptions")]
    public async Task Exceptions()
    {
        if (string.IsNullOrEmpty(_exceptionStrings))
        {
            _exceptionStrings = ExceptionDetailClient.FromAssemblies(SharecodeRestApi.ReferencingAssemblies).CollectErrors(typeof(AppException))
                .ToString();
        }
        
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        await Response.WriteAsync(_exceptionStrings);
        return;
    }
}