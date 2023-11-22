using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Events.Users;

public sealed record UserCreatedDomainEvent(
    Guid UserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string EmailAddress
) : UserEvent(UserId, FullName, EmailAddress)
{

    public static UserCreatedDomainEvent Create(User user)
    {
        return new UserCreatedDomainEvent(user.Id, user.FirstName, user.MiddleName, user.LastName, user.FullName, user.EmailAddress);
    }
    
}