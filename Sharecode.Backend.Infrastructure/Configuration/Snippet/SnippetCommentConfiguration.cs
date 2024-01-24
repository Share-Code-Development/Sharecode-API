using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetCommentConfiguration : IEntityTypeConfiguration<SnippetComment>
{
    public void Configure(EntityTypeBuilder<SnippetComment> builder)
    {

        builder.ToTable("SnippetComments", "snippet");

        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(1000);


        builder.Property(x => x.Mentions)
            .HasColumnType("jsonb");
        
        
        #region ForeginKey
        //Link comment to snippet
        builder.HasOne(x => x.Snippet)
            .WithMany()
            .HasForeignKey(x => x.SnippetId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //Link child comments to parent comments
        builder.HasOne<SnippetComment>(x => x.ParentComment)
            .WithMany()
            .HasForeignKey(x => x.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);

        //Link the comment to the user
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion

        #region Index

        builder.HasIndex(p => p.IsDeleted)
            .HasFilter("\"IsDeleted\" = true");

        //To get based on the snippet
        builder.HasIndex(x => x.SnippetId)
            .HasMethod("BTREE");

        //To get all the users comment
        builder.HasIndex(x => x.UserId)
            .HasMethod("BTREE");

        //To get the responses of a comment
        builder.HasIndex(x => new {x.ParentCommentId, x.SnippetId})
            .HasMethod("BTREE");

        #endregion

    }
}