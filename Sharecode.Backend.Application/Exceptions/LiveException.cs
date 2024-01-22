using System.Net;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Application.Exceptions;

[ExceptionDetail(76223, "An unknown error occured while handling the live request")]
public class LiveException : AppException
{
    public LiveException(string message, string? extendedMessage = default, string? eventType = default) : base(message, 76223, HttpStatusCode.InternalServerError, extendedMessage)
    {
        if (string.IsNullOrEmpty(eventType))
        {
            SetMessage($"Your request failed to specify the event type");
        }
        else
        {
            SetMessage($"An unknown error occured while processing the event of {eventType}. Occured exception is : {message}. Additional Message: {extendedMessage}");
        }
    }
}