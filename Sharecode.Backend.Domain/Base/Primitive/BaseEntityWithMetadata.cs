namespace Sharecode.Backend.Domain.Base.Primitive;

public abstract class BaseEntityWithMetadata : BaseEntity
{
    public List<Meta> Metadata { get; set; } = new List<Meta>();

}

