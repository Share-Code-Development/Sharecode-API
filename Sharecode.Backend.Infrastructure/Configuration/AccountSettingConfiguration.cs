using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class AccountSettingConfiguration : IEntityTypeConfiguration<AccountSetting>
{
    public void Configure(EntityTypeBuilder<AccountSetting> builder)
    {
        builder.OwnsOne(x => x.Metadata, b => b.ToJson());
    }
}