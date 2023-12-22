using System.Net;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Infrastructure.Exceptions;

[ExceptionDetail(errorCode: 46393, errorDescription: "This error defines some kind of infrastructural problem with Sharecode")]
public class InfrastructureDownException : AppException
{
    public InfrastructureDownException(string? message, string? extendedMessage) : base(message, 46393, HttpStatusCode.InternalServerError, extendedMessage: extendedMessage)
    {
        SetMessage($"Seems like we are down!");
    }
}