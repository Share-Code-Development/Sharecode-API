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

namespace Sharecode.Backend.Api.Controller;

[EnableRateLimiting("fixed")]
public class CommonController(IDistributedCache cache, IHttpClientContext requestContext, ILogger<AbstractBaseEndpoint> logger, IMediator mediator) : AbstractBaseEndpoint(cache, requestContext, logger, mediator)
{

    private static string _exceptionStrings = string.Empty;
    
    [HttpGet]
    public ActionResult Health()
    {
        return Ok();
    }

    [HttpGet("exceptions")]
    public async Task Exceptions()
    {
        if (string.IsNullOrEmpty(_exceptionStrings))
        {
            _exceptionStrings = ExceptionDetailClient.FromAssemblies(Sharecode.ReferencingAssemblies).CollectErrors(typeof(AppException))
                .ToString();
        }
        
        Response.ContentType = "application/json";
        Response.StatusCode = 200;
        await Response.WriteAsync(_exceptionStrings);
        return;
    }
}