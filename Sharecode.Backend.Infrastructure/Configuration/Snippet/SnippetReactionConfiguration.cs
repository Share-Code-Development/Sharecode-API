using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetReactionConfiguration : IEntityTypeConfiguration<SnippetReactions>
{
    public void Configure(EntityTypeBuilder<SnippetReactions> builder)
    {
        builder.HasIndex(x => new { x.SnippetId, x.UserId })
            .IsUnique();
        
        builder.HasIndex(p => p.IsDeleted)
            .HasFilter("\"IsDeleted\" = true");

        builder.HasIndex(x => x.SnippetId)
            .HasMethod("BTREE");
        
        
    }
}