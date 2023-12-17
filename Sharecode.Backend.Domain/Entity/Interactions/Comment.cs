using System.ComponentModel.DataAnnotations;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Domain.Entity.Interactions;

public class Comment : BaseEntity
{
    [Required]
    [Length(minimumLength: 0, maximumLength:500)]
    public string Text { get; set; }
}