using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Domain.Entity.Profile;

public class UserRefreshToken : BaseEntity
{

    [NotMapped] public new Guid Id => TokenIdentifier;
    [Required] [Key] public Guid TokenIdentifier { get; init; }
    public Guid IssuedFor { get; init; }
    public bool IsValid { get; private set; }
    

    public override void SoftDeleteEntity()
    {
        HardDeleteEntity();
    }
}