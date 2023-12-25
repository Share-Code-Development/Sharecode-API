using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;
using Sharecode.Backend.Infrastructure.Db.Extensions;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetConfiguration : IEntityTypeConfiguration<Domain.Entity.Snippet.Snippet>
{
    public void Configure(EntityTypeBuilder<Domain.Entity.Snippet.Snippet> builder)
    {
        builder.ToTable("Snippets", "snippet");
        builder.Property(x => x.Metadata)
            .HasColumnType("jsonb")
            .HasJsonConversion();
        
        builder.ToTable(x => x.HasCheckConstraint("CK_Snippet_Meta_Ensure_Json", "\"Metadata\"::jsonb IS NOT NULL"));
        builder.Property(x => x.Tags)
            .HasColumnType("jsonb")
            .HasJsonConversion();
        
        builder.ToTable(x => x.HasCheckConstraint("CK_Snippet_Tags_Ensure_Json", "\"Tags\"::jsonb IS NOT NULL"));

        builder.HasMany<SnippetComment>(s => s.Comments)
            .WithOne(x => x.Snippet)
            .HasForeignKey(x => x.SnippetId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany<SnippetReactions>(s => s.Reactions)
            .WithOne(x => x.Snippet)
            .HasForeignKey(x => x.SnippetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<SnippetAccessControl>(x => x.AccessControls)
            .WithOne(x => x.Snippet)
            .HasForeignKey(x => x.SnippetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .IsRequired(false);

        builder.Property(x => x.Title)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Language)
            .HasMaxLength(20);

        builder.Property(x => x.PreviewCode)
            .HasMaxLength(500);

        builder.Property(x => x.CheckSum)
            .HasColumnType("bytea")
            .IsRequired();
    }
}