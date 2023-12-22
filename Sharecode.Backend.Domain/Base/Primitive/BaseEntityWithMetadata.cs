namespace Sharecode.Backend.Domain.Base.Primitive;

public abstract class BaseEntityWithMetadata : BaseEntity
{
    public Dictionary<string, string> Metadata { get; set; } = new();
}

