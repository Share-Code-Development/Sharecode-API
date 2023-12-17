using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetCommentReactionConfiguration : IEntityTypeConfiguration<SnippetCommentReactions>
{
    public void Configure(EntityTypeBuilder<SnippetCommentReactions> builder)
    {
        builder.HasIndex(x => new { x.UserId, x.SnippetCommentId })
            .IsUnique();
        
        builder.HasIndex(p => p.IsDeleted)
            .HasFilter("\"IsDeleted\" = true");

        builder.HasIndex(x => x.SnippetCommentId)
            .HasMethod("BTREE");
    }
}