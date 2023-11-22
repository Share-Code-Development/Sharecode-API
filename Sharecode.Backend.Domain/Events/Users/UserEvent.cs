using Sharecode.Backend.Domain.Base;

namespace Sharecode.Backend.Domain.Events.Users;

public record UserEvent(    
    Guid UserId,
    string FullName,
    string EmailAddress) : IDomainEvent;