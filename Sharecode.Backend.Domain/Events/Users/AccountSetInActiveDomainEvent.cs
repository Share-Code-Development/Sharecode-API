using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Events.Users;

public record AccountSetInActiveDomainEvent(Guid UserId, string FullName, string EmailAddress, string Reason) : UserEvent(UserId, FullName, EmailAddress)
{
    public static AccountSetInActiveDomainEvent Create(User user)
    {
        return new AccountSetInActiveDomainEvent(user.Id, user.FullName, user.EmailAddress, user.InActiveReason!);
    }
}