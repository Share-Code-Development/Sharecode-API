using System.Net;
using System.Runtime.Serialization;

namespace Sharecode.Backend.Domain.Exceptions;

public abstract class AppException : Exception
{
    public readonly long ErrorCode;
    public readonly HttpStatusCode StatusCode;
    public readonly List<object> Errors = new List<object>();
    public string PublicMessage = string.Empty;

    protected AppException(long errorCode, HttpStatusCode statusCode)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    [Obsolete("Obsolete")]
    protected AppException(SerializationInfo info, StreamingContext context, long errorCode, HttpStatusCode statusCode) : base(info, context)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    protected AppException(string? message, long errorCode, HttpStatusCode statusCode) : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    protected AppException(string? message, Exception? innerException, long errorCode, HttpStatusCode statusCode) : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
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