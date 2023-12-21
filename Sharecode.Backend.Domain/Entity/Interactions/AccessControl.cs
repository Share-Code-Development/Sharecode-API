using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Entity.Interactions;

public abstract class AccessControl<TEntity> : BaseEntityWithMetadata
{
    public bool Read { get; private set; }
    public bool Write { get; private set; }
    public bool Manage { get; private set; }
    
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    protected void CreateForOwner(Guid userId)
    {
        UserId = userId;
        Read = Write = Manage = true;
    }

    public abstract void SetOwnership(TEntity entity);
}

