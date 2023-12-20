using System.ComponentModel.DataAnnotations;
using Sharecode.Backend.Domain.Base.Primitive;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Domain.Entity.Interactions;

public class Comment : BaseEntity
{
    [Required]
    [Length(minimumLength: 0, maximumLength:1000)]
    public string Text { get; set; }
    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; }
}