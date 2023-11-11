using System.ComponentModel.DataAnnotations;

namespace Sharecode.Backend.Domain.Base;

public class MetadataHolder
{
    [Required]
    public Dictionary<string, object> Document { get; private set; } = new Dictionary<string, object>();
}

public class Metadata
{
    public string Key { get; set; }
    public string Value { get; set; }
} 