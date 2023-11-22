using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Events.Users;

public record UserVerifiedDomainEvent(
    Guid UserId,
    string FirstName,
    string? MiddleName,
    string LastName,
    string FullName,
    string EmailAddress
) : UserEvent(UserId, FullName, EmailAddress)
{
    public static UserVerifiedDomainEvent Create(User user)
    {
        return new UserVerifiedDomainEvent(user.Id, user.FirstName, user.MiddleName, user.LastName, user.FullName, user.EmailAddress);
    }
}