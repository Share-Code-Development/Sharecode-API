using System.Net;
using System.Runtime.Serialization;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Utilities.JsonExceptions;

namespace Sharecode.Backend.Domain.Exceptions;

[ExceptionDetail(errorCode: 10212, errorDescription: "The server failed to find the request entity")]
public class EntityNotFoundException : AppException
{
    public Type EntityType { get; private set; }
    public object EntityIdentifier { get; private set; }
    public EntityNotFoundException(Type entityType, object identifier, bool showIdentifier = false) : base(showIdentifier ? $"The requested {entityType.Name} with identifier {identifier} is not found" : $"The requested [ENTITY_TYPE] [ENTITY_VALUE] is not found",10212, HttpStatusCode.NotFound)
    {
        EntityType = entityType;
        EntityIdentifier = identifier;
        SetMessage(showIdentifier ? $"The requested {entityType.Name} with identifier {identifier} is not found" : $"The requested [ENTITY_TYPE] [ENTITY_VALUE] is not found");
    }
    
}