using System.ComponentModel.DataAnnotations;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Domain.Entity.Interactions;

public class Reactions : BaseEntity
{
    [Required]
    public string ReactionType { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}