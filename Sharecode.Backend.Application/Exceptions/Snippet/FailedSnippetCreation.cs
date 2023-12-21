using System.Net;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Application.Exceptions.Snippet;
[ExceptionDetail(errorCode: 238123, errorDescription: "Failed to create an exception")]
public class FailedSnippetCreation : AppException
{
    public FailedSnippetCreation(string? message) : base(message, 238123, HttpStatusCode.BadRequest)
    {
        SetMessage($"Failed to create the snippet due to : {message}");
    }
}