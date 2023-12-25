using Sharecode.Backend.Domain.Base;

namespace Sharecode.Backend.Domain.Events.Users
{
    public class DeleteUserDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public Guid RequestedBy { get; }
        public bool SoftDelete { get; }

        // Constructor
        public DeleteUserDomainEvent(Guid userId, Guid requestedBy, bool softDelete)
        {
            this.UserId = userId;
            this.RequestedBy = requestedBy;
            this.SoftDelete = softDelete;            
        }
    }
}