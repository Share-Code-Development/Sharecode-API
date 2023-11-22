using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sharecode.Backend.Domain.Base.Primitive;

public abstract class BaseEntity
{

    [NotMapped]
    [JsonIgnore]
    private readonly List<IDomainEvent> _domainEvents = new();
    [Key] [Required] [DatabaseGenerated(DatabaseGeneratedOption.Identity)] public Guid Id { get; set; }
    [Required] public DateTime CreatedAt { get; set; } = DateTime.Now;
    [Required] public DateTime ModifiedAt { get; set; } 
    [Timestamp] public byte[] Version { get; set; } = null!;
    [Required] public bool IsDeleted { get; private set; } = false;
    [NotMapped] public bool HardDelete { get; private set; } = false;
    
    protected void RaiseDomainEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public virtual void RaiseCreatedEvent()
    {
        //Implement by the child entity
    }

    protected virtual void RaiseDeletedEvent()
    {
        //Implement by the child entity
    }
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.ToList();

    public void ClearEvents()
    {
        _domainEvents.Clear();
    }
    
    public virtual void SoftDeleteEntity()
    {
        ModifiedAt = DateTime.UtcNow;
        IsDeleted = true;
        RaiseDeletedEvent();
    }

    public virtual void HardDeleteEntity()
    {
        HardDelete = true;
    }
}