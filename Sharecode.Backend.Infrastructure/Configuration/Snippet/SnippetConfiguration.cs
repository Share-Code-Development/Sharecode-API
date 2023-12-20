using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetConfiguration : IEntityTypeConfiguration<Domain.Entity.Snippet.Snippet>
{
    public void Configure(EntityTypeBuilder<Domain.Entity.Snippet.Snippet> builder)
    {
        builder.ToTable("Snippets", "snippet");
        builder.OwnsOne(x => x.Metadata, b => b.ToJson());
        builder.ToTable(x => x.HasCheckConstraint("CK_Snippet_Meta_Ensure_Json", "\"Metadata\"::jsonb IS NOT NULL"));
        
        builder.OwnsOne(x => x.Tags, b => b.ToJson());
        builder.ToTable(x => x.HasCheckConstraint("CK_Snippet_Tags_Ensure_Json", "\"Tags\"::jsonb IS NOT NULL"));

        builder.HasMany<SnippetComment>(s => s.Comments)
            .WithOne(x => x.Snippet)
            .HasForeignKey(x => x.SnippetId);
        
        builder.HasMany<SnippetReactions>(s => s.Reactions)
            .WithOne(x => x.Snippet)
            .HasForeignKey(x => x.SnippetId);
    }
}