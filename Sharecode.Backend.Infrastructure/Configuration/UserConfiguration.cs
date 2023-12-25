using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Entity.Profile;
using Sharecode.Backend.Domain.Enums;
using Sharecode.Backend.Infrastructure.Db.Extensions;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        
        #region Index

        builder.HasIndex(x => x.EmailAddress).IsUnique();

        builder.HasIndex(x => x.NormalizedFullName)
            .HasMethod("btree");
        
        builder.HasIndex(x => x.Permissions)
            .HasMethod("GIN");
        
        // Partial index for IsDeleted
        builder.HasIndex(p => p.IsDeleted)
            .HasFilter("\"IsDeleted\" = true");

        builder.HasIndex(p => p.EmailVerified)
            .HasFilter("\"EmailVerified\" = true");

        #endregion
        
        #region Json

        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb")
            .HasJsonConversion();
        
        builder.ToTable(x => x.HasCheckConstraint("CK_User_Ensure_Json", "\"Metadata\"::jsonb IS NOT NULL"));

        builder.Property(x => x.Permissions)
            .HasColumnType("jsonb")
            .HasJsonConversion();
        
        builder.ToTable(x => x.HasCheckConstraint("CK_User_Ensure_Perms_Json", "\"Permissions\"::jsonb IS NOT NULL"));

        #endregion

        #region ForiegnKey

        builder.HasOne(user => user.AccountSetting)
            .WithOne(accountSettings => accountSettings.User)
            .HasForeignKey<AccountSetting>(x => x.UserId)
            .IsRequired();
        

        #endregion

        #region Constraints

        //Length Setting
        builder.Property(x => x.EmailAddress)
            .HasMaxLength(100);
        
        builder.Property(x => x.FirstName)
            .HasMaxLength(100);
        
        builder.Property(x => x.MiddleName)
            .HasMaxLength(100);        
        
        builder.Property(x => x.LastName)
            .HasMaxLength(100);     
        
        builder.Property(x => x.NormalizedFullName)
            .HasMaxLength(300);

        builder.Property(x => x.EmailVerified)
            .HasDefaultValue(false);

        builder.Property(x => x.Visibility)
            .HasDefaultValue(AccountVisibility.Public);

        builder.Property(x => x.IsDeleted)
            .HasDefaultValue(false);

        #endregion

    }
}