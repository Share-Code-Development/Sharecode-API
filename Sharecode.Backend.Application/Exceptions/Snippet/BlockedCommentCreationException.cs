using System.Net;
using Sharecode.Backend.Domain.Exceptions;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Application.Exceptions.Snippet;

[ExceptionDetail(errorCode: 31900, errorDescription: "The user cannot create comment on the requested snippet since he is been restricted to create")]
public class BlockedCommentCreationException()
    : AppException($"You cannot create a comment on this post", 31900, HttpStatusCode.BadRequest);