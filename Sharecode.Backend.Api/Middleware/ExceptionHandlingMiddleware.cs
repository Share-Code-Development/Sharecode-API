using Sharecode.Backend.Application.Exceptions;
using Sharecode.Backend.Infrastructure.Exceptions.Jwt;

namespace Sharecode.Backend.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An exception has been caught on the request pipeline. Reason: {Message}. Stacktrace: {StackTrace}", e.Message, e.StackTrace);
            ExceptionDetail exceptionDetail = BuildExceptionMessage(e, _env.IsDevelopment());
            context.Response.StatusCode = exceptionDetail.Code;
            await context.Response.WriteAsJsonAsync(exceptionDetail);
        }
    }

    private static ExceptionDetail BuildExceptionMessage(Exception exception, bool showExtended = false)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetail(
                StatusCodes.Status400BadRequest,
                "Validation Error",
                "Failed to validate properties provided",
                validationException.Errors
            ),
            JwtGenerationException jwtGenerationException => new ExceptionDetail(
                StatusCodes.Status500InternalServerError,
                "Access Generation Failure",
                $"Failed to generate access tokens.",
                null,
                ExtendedMessage: showExtended ? jwtGenerationException.Message : ""
                ),
            JwtFetchKeySecretException jwtFetchKeySecretException => new ExceptionDetail(
                StatusCodes.Status500InternalServerError,
                "Access Generation Failure",
                $"Failed to fetch keys. {jwtFetchKeySecretException.Message}",
                null,
                ExtendedMessage: showExtended ? jwtFetchKeySecretException.Message : ""
                ),
            _ => new ExceptionDetail(
                StatusCodes.Status500InternalServerError,
                "Unknown Error",
                "An unknown error occured. Please retry again",
                null
            )
        };
    }
}


internal sealed record ExceptionDetail(int Code, string Type, string Message, IEnumerable<object>? Errors, string? ExtendedMessage = null);