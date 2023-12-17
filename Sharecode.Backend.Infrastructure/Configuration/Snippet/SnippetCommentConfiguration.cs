using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetCommentConfiguration : IEntityTypeConfiguration<SnippetComment>
{
    public void Configure(EntityTypeBuilder<SnippetComment> builder)
    {
        builder.HasMany<SnippetCommentReactions>(x => x.Reactions)
            .WithOne(x => x.SnippetComment)
            .HasForeignKey(x => x.SnippetCommentId);
        
        builder.HasIndex(p => p.IsDeleted)
            .HasFilter("\"IsDeleted\" = true");

        builder.HasIndex(x => x.SnippetId)
            .HasMethod("BTREE");
    }
}