using System.Net;
using System.Runtime.Serialization;

namespace Sharecode.Backend.Domain.Exceptions;

public class NoAccessException : AppException
{

    public NoAccessException(object requestUserIdentifier, object accessingIdentifier, Type type) : base($"{requestUserIdentifier} doesn't have right to access {accessingIdentifier}[{type.Name}]", 34181,
        HttpStatusCode.Forbidden)
    {
        SetMessage($"{requestUserIdentifier} doesn't have right to access {accessingIdentifier}[{type.Name}]");
    }

}