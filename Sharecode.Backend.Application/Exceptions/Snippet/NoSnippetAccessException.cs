using System.Net;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Application.Exceptions.Snippet;

[ExceptionDetail(errorCode: 934134, errorDescription: "This error is thrown when a user try to access snippet which is not public or he doesn't have access to")]
public class NoSnippetAccessException : AppException
{
    public NoSnippetAccessException(Guid snippetId) : base($"No access to the snippet {snippetId.ToString()}", 934134, HttpStatusCode.Forbidden)
    {
        SetMessage($"This snippet is either private or you don't have access to this snippet");
    }
}