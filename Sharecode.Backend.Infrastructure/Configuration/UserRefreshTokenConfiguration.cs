using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Profile;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasIndex(x => x.IssuedFor).HasMethod("btree");
        builder.Property(x => x.IssuedFor).IsRequired();
        builder.HasKey(t => t.TokenIdentifier);

        builder
            .HasOne<User>() // Assuming there is a User entity
            .WithMany() // No corresponding navigation property in User class
            .HasForeignKey(t => t.IssuedFor)
            .OnDelete(DeleteBehavior.Cascade);

    }
}