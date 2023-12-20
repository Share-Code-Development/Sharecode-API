using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Entity.Interactions;

public class AccessControl : BaseEntityWithMetadata
{
    public bool Read { get; private set; }
    public bool Write { get; private set; }
    public bool Manage { get; private set; }
    
    public Guid UserId { get; private set; }
    public User User { get; private set; }
}

