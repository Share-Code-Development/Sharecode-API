using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Infrastructure.Db.Extensions;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class AccountSettingConfiguration : IEntityTypeConfiguration<AccountSetting>
{
    public void Configure(EntityTypeBuilder<AccountSetting> builder)
    {
        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb")
            .HasJsonConversion();
        
        builder.ToTable(x => x.HasCheckConstraint("CK_AccountSetting_Ensure_Json", "\"Metadata\"::jsonb IS NOT NULL"));

        builder.Property(x => x.EnableNotificationsForMentions)
            .IsRequired(true)
            .HasDefaultValue(true);
    }
}