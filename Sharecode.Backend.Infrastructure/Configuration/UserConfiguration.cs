﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity;
using Sharecode.Backend.Domain.Enums;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //Create unique indexes
        builder.HasIndex(x => x.EmailAddress).IsUnique();
        /*builder.Property(x => x.FullName)
            .HasComputedColumnSql(@"CASE 
            WHEN MiddleName IS NULL OR MiddleName = '' THEN FirstName + ' ' + LastName
            ELSE FirstName + ' ' + MiddleName + ' ' + LastName
            END"
                );*/
        builder.HasIndex(x => x.NormalizedFullName)
            .IsClustered(false);
        
        //Configure the meta data column to be a JSON
        builder.OwnsOne(x => x.Metadata, b => b.ToJson());

        builder.HasOne(user => user.AccountSetting)
            .WithOne(accountSettings => accountSettings.User);
        
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
    }
}