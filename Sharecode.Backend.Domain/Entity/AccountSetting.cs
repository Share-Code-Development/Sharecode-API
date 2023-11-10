using System.ComponentModel.DataAnnotations;
using Sharecode.Backend.Domain.Base;

namespace Sharecode.Backend.Domain.Entity;

public class AccountSetting : BaseEntity
{
    [Required] public Guid UserId { get; set; }
    [Required] public User User { get; set; }
    [Required] public bool AllowTagging { get; set; } = false;
}