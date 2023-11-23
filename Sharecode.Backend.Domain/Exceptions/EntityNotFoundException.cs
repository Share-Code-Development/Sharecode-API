using System.Net;
using System.Runtime.Serialization;

namespace Sharecode.Backend.Domain.Exceptions;

public class EntityNotFoundException : AppException
{
    
    public Type EntityType { get; private set; }
    public object EntityIdentifier { get; private set; }
    public EntityNotFoundException(Type entityType, object identifier) : base($"The requested {entityType.Name} with identifier {identifier} is not found",10212, HttpStatusCode.NotFound)
    {
        EntityType = entityType;
        EntityIdentifier = identifier;
        SetMessage($"The requested {entityType.Name.ToLower()} with identifier {identifier} is not found");
    }
    
}