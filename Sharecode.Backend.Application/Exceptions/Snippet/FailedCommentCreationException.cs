using System.Net;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Application.Exceptions.Snippet;

[ExceptionDetail(errorCode: 291910, errorDescription: "")]
public class FailedCommentCreationException : AppException
{
    public FailedCommentCreationException(string? message) : base(message, 291910, HttpStatusCode.BadRequest)
    {
        SetMessage(message);
    }
}