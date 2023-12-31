﻿using System.ComponentModel.DataAnnotations;
using Sharecode.Backend.Domain.Base;
using Sharecode.Backend.Domain.Base.Primitive;

namespace Sharecode.Backend.Domain.Entity.Profile;

public class AccountSetting : BaseEntityWithMetadata
{
    [Required] public Guid UserId { get; set; }
    [Required] public User User { get; set; } = null!;
    [Required] public bool AllowTagging { get; set; } = false;
    [Required] public bool EnableNotificationsForMentions { get; set; } = true;
}