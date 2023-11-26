using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Domain.Entity.Profile;

public class UserPasswordTracker : BaseEntity
{
    public User User { get; set; }
    public Guid UserId { get; set; }
    public byte[]? Salt { get; private set; }
    public byte[]? PasswordHash { get; private set; }
}