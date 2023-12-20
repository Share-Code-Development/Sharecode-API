using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetCommentReactionConfiguration : IEntityTypeConfiguration<SnippetCommentReactions>
{
    public void Configure(EntityTypeBuilder<SnippetCommentReactions> builder)
    {
        
        builder.ToTable("SnippetCommentReactions", "snippet");

        #region Constraints

        builder.Property(x => x.ReactionType)
            .HasMaxLength(20)
            .IsRequired();

        #endregion
        
        #region Index

        builder.HasIndex(x => new { x.UserId, x.SnippetCommentId })
            .IsUnique();
        
        builder.HasIndex(p => p.IsDeleted)
            .HasFilter("\"IsDeleted\" = true");

        builder.HasIndex(x => x.SnippetCommentId)
            .HasMethod("BTREE");
        
        builder.HasIndex(x => x.ReactionType)
            .HasMethod("HASH");

        #endregion
        
        #region ForeignKeys

        /*builder.HasOne(x => x.SnippetComment)
            .WithMany()
            .HasForeignKey(x => x.SnippetCommentId)
            .IsRequired();*/
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        #endregion
    }
}