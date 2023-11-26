using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Events.Users;

public record RequestPasswordResetDomainEvent(Guid UserId, string FullName, string EmailAddress) : UserEvent(UserId,
    FullName, EmailAddress)
{
    public static RequestPasswordResetDomainEvent Create(User user)
    {
        return new RequestPasswordResetDomainEvent(user.Id, user.FullName, user.EmailAddress);
    }
}