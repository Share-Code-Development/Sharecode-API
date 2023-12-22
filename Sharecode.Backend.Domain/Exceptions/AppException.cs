using System.Net;
using System.Runtime.Serialization;

namespace Sharecode.Backend.Domain.Exceptions;


public abstract class AppException : Exception
{
    public readonly long ErrorCode;
    public readonly HttpStatusCode StatusCode;
    public readonly List<object> Errors = new List<object>();
    public string PublicMessage;
    public readonly string? ExtendedMessage;

    protected AppException(string? message, long errorCode, HttpStatusCode statusCode, string? extendedMessage = null) : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
        PublicMessage = message ?? String.Empty;
        ExtendedMessage = extendedMessage;
    }

    public AppException AddError(object error)
    {
        Errors.Add(error);
        return this;
    }

    public AppException SetMessage(string? exceptionPublicMessage)
    {
        if (exceptionPublicMessage == null)
            return this;
        
        PublicMessage = exceptionPublicMessage;
        return this;
    }
}