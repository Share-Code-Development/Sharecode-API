using System.ComponentModel.DataAnnotations;

namespace Sharecode.Backend.Domain.Base;

public abstract class BaseEntityWithMetadata : BaseEntity
{
    [Required] public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

}

