using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class AccountSettingConfiguration : IEntityTypeConfiguration<AccountSetting>
{
    public void Configure(EntityTypeBuilder<AccountSetting> builder)
    {
        builder.OwnsOne(x => x.Metadata, b => b.ToJson());
        builder.ToTable(x => x.HasCheckConstraint("CK_AccountSetting_Ensure_Json", "ISJSON([Metadata]) > 0"));

    }
}