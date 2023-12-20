using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetReactionConfiguration : IEntityTypeConfiguration<SnippetReactions>
{
    public void Configure(EntityTypeBuilder<SnippetReactions> builder)
    {
        builder.ToTable("SnippetReactions", "snippet");
        
        builder.Property(x => x.ReactionType)
            .HasMaxLength(20)
            .IsRequired();

        #region ForeignKeys
        //Links the reaction to the snippet
        /*builder.HasOne(x => x.Snippet)
            .WithMany()
            .HasForeignKey(x => x.SnippetId)
            .IsRequired();*/
        
        //Links the reaction to the user
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        #endregion

        #region Indexes

        builder.HasIndex(x => new { x.SnippetId, x.UserId })
            .HasMethod("BTREE")
            .IsUnique();
        
        builder.HasIndex(p => p.IsDeleted)
            .HasFilter("\"IsDeleted\" = true");

        builder.HasIndex(x => x.SnippetId)
            .HasMethod("BTREE");

        //To get the count of the comments of a particular type
        builder.HasIndex(x => x.ReactionType)
            .HasMethod("HASH");

        #endregion

    }
}