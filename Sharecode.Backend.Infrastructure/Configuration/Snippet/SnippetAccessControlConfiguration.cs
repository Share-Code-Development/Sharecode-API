using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Snippet;

namespace Sharecode.Backend.Infrastructure.Configuration.Snippet;

public class SnippetAccessControlConfiguration : IEntityTypeConfiguration<SnippetAccessControl>
{
    public void Configure(EntityTypeBuilder<SnippetAccessControl> builder)
    {
        builder.ToTable("SnippetAccessControls", "snippet");

        #region ForeignKeys
        //Link to snippets
        /*builder.HasOne(x => x.Snippet)
            .WithMany()
            .HasForeignKey(x => x.SnippetId)
            .IsRequired();*/

        //Link to users
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();
        #endregion

        
        #region Constraints
        
        //Always ensure meta data is a Json
        /*builder.OwnsOne(x => x.Metadata, b =>
        {
            b.ToJson();
        });*/
        builder.ToTable(x => x.HasCheckConstraint("CK_SAC_Ensure_Json", "\"Metadata\"::jsonb IS NOT NULL"));

        builder.HasIndex(x => new { x.UserId, x.SnippetId })
            .IsUnique();

        #endregion

    }
}