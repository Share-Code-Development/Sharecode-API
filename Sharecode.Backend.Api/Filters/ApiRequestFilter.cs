using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sharecode.Backend.Api.Attributes;
using Sharecode.Backend.Application.Client;

namespace Sharecode.Backend.Api.Filters;

public class ApiRequestFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var httpClientContext = context.HttpContext.RequestServices.GetService<IHttpClientContext>();
        var allowApiRequest = context.ActionDescriptor.EndpointMetadata
            .OfType<AllowApiRequestAttribute>()
            .Any();

        if (!allowApiRequest && httpClientContext?.IsApiRequest == true)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
        
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Implementation if needed
    }
}