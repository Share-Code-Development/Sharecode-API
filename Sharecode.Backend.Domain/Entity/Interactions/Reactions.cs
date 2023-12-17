using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Domain.Entity.Interactions;

public class Reactions : BaseEntity
{
    public ReactionTypes Type { get; set; }
    public Guid UserId { get; set; }
}