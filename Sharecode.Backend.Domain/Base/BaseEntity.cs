using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sharecode.Backend.Domain.Base;

public abstract class BaseEntity
{
    [Key] [Required] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public Guid Id { get; set; }
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required] public DateTime ModifiedAt { get; set; } 
    [Timestamp] public byte[] Version { get; set; }
    [Required] public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    [Required] public bool IsDeleted { get; set; } = false;
    public void SetUpdated()
    {
        this.ModifiedAt = DateTime.Now;
    }
}