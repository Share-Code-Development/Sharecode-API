using System.Text.Json.Serialization;
using Microsoft.AspNetCore.SignalR;
using Sharecode.Backend.Api.Exceptions;
using Sharecode.Backend.Api.Middleware.Http;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.Extensions;
using ILogger = Serilog.ILogger;

namespace Sharecode.Backend.Api.Middleware.SignalR;

public class ExceptionFilter(ILogger logger) : IHubFilter
{
    /*
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(invocationContext);
        }
        catch (AggregateException aggEx)
        {
            var innerException = aggEx.Flatten().InnerException;
            logger.Error(innerException, "An aggregate exception has been caught on the request pipeline.");
            var exception = await HandleExceptionAsync(invocationContext.Context.GetHttpContext(), innerException);
            //await invocationContext.Hub.Clients.Caller.SendAsync(exception);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An exception has been caught on the request pipeline.");
            await HandleExceptionAsync(invocationContext.Context.GetHttpContext(), ex);
        }
        
        return 
    }
    */
    
    private async Task<LiveEvent<LiveException>> HandleExceptionAsync(HttpContext? context, Exception? exception)
    {
        LiveException exceptionDetail = BuildExceptionMessage(exception);
        return new LiveEvent<LiveException>(exceptionDetail);
    }
    
        private static LiveException BuildExceptionMessage(Exception? exception, bool showExtended = false)
    {
        return exception switch
        {
            UnauthorizedAccessException unauthorizedAccessException => new LiveException(
                StatusCodes.Status401Unauthorized,
                "Unauthorized access",
                unauthorizedAccessException.Message,
                null,
                null
                ),
            BadHttpRequestException badHttpRequestException => new LiveException(
                StatusCodes.Status400BadRequest,
                Type: "The request is not valid",
                Message: badHttpRequestException.Message,
                Errors: null,
                ExtendedMessage: showExtended ? badHttpRequestException.InnerException?.Message : ""
                ),
            AppException appException => CreateExceptionDetail(appException),
            { } ex => new LiveException(
                StatusCodes.Status500InternalServerError,
                "Unknown Error",
                "An unknown error occured. Please retry again",
                [ex.Message]
            ),
            _ => new LiveException(
                StatusCodes.Status500InternalServerError,
                "Unknown Error",
                "An unknown error occured. Please retry again",
                null
            ),
        };
    }
    
    private static LiveException CreateExceptionDetail(AppException appException)
    {
        string typeName = appException.GetType().Name;
        string readableName = ExceptionCache.Exceptions.GetOrAdd(typeName, key =>
            key.ToCapitalized().Replace("Exception", string.Empty).TrimEnd()
        );
        

        return new LiveException(
            (int)appException.StatusCode,
            readableName,
            appException.PublicMessage == String.Empty ? appException.Message : appException.PublicMessage,
            appException.Errors, 
            appException.ExtendedMessage ?? appException.InnerException?.Message,
            ErrorCode: appException.ErrorCode
        );
    }
}

internal sealed class LiveException(
    [property: Newtonsoft.Json.JsonIgnore] [field: Newtonsoft.Json.JsonIgnore] [property: JsonIgnore] [field: JsonIgnore] int StatusCode,
    string Type, string Message, 
    IEnumerable<object>? Errors, 
    string? ExtendedMessage = null,
    long? ErrorCode = null
);