using System.Net;
using System.Runtime.Serialization;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Domain.Exceptions;

[ExceptionDetail(errorCode: 34181, "The user doesn't have privilege to access the requested entity(s)")] //No Access
public class NoAccessException : AppException
{

    public NoAccessException(object requestUserIdentifier, object accessingIdentifier, Type type) : base($"{requestUserIdentifier} doesn't have right to access {accessingIdentifier}[{type.Name}]", 34181,
        HttpStatusCode.Forbidden)
    {
        SetMessage($"{requestUserIdentifier} doesn't have right to access {accessingIdentifier}[{type.Name}]");
    }
    
    public NoAccessException(object requestUserIdentifier, object accessingIdentifier, string type) : base($"{requestUserIdentifier} doesn't have right to access {accessingIdentifier}[{type}]", 34181,
        HttpStatusCode.Forbidden)
    {
        SetMessage($"{requestUserIdentifier} doesn't have right to access {accessingIdentifier}[{type}]");
    }

}