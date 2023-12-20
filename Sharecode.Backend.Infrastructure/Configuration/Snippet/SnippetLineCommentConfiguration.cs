using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetLineCommentConfiguration : IEntityTypeConfiguration<SnippetLineComment>
{
    public void Configure(EntityTypeBuilder<SnippetLineComment> builder)
    {
        builder.ToTable("SnippetLineComments", "snippet");
        builder.Property(x => x.Text)
            .IsRequired()
            .HasMaxLength(1000);
        
        #region ForeginKey
        //Link comment to snippet
        builder.HasOne(x => x.Snippet)
            .WithMany()
            .HasForeignKey(x => x.SnippetId)
            .IsRequired();

        //Link the line comment to user
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();
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
        
        #endregion
    }
}