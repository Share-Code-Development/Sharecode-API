using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Utilities.RequestDetail;

namespace Sharecode.Backend.Domain.Events.Users;

public record AccountLockedDomainEvent(Guid UserId, string FullName, string EmailAddress, IRequestDetail RequestDetail, DateTime LastOccurence) : UserEvent(UserId, FullName, EmailAddress)
{
    public static AccountLockedDomainEvent Create(User user, IRequestDetail requestDetail)
    {
        return new AccountLockedDomainEvent(user.Id, user.FullName, user.EmailAddress, requestDetail, DateTime.UtcNow);
    }
}