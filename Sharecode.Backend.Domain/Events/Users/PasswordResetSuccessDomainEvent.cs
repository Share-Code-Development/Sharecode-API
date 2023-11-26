using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Utilities.RequestDetail;

namespace Sharecode.Backend.Domain.Events.Users;

public record PasswordResetSuccessDomainEvent(Guid UserId, string FullName, string EmailAddress, DateTime ResetAt, IRequestDetail RequestDetail, bool WasAccountLocked) : UserEvent(UserId, FullName, EmailAddress)
{
    public static PasswordResetSuccessDomainEvent Create(User user, IRequestDetail requestDetail, bool wasAccountLocked)
    {
        return new PasswordResetSuccessDomainEvent(user.Id, user.FullName, user.EmailAddress, DateTime.UtcNow, requestDetail, wasAccountLocked);
    }
}