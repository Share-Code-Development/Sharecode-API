using System.Net;
using System.Runtime.Serialization;
using Sharecode.Backend.Domain.Exceptions;

namespace Sharecode.Backend.Application.Exceptions;

public class UserNotFoundException : AppException
{
    public readonly Guid UserId;
    public UserNotFoundException(long errorCode, Guid userId) : base($"Failed to find the user {userId}", errorCode, HttpStatusCode.NotFound)
    {
        UserId = userId;
    }

    public UserNotFoundException(SerializationInfo info, StreamingContext context, long errorCode, Guid userId) : base(info, context, errorCode, HttpStatusCode.NotFound)
    {
        UserId = userId;
    }

    public UserNotFoundException(string? message, long errorCode, Guid userId) : base(message, errorCode, HttpStatusCode.NotFound)
    {
        UserId = userId;
    }

    public UserNotFoundException(string? message, Exception? innerException, long errorCode, Guid userId) : base(message, innerException, errorCode, HttpStatusCode.NotFound)
    {
        UserId = userId;
    }
}