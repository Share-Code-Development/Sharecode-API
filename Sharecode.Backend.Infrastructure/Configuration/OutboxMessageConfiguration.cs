using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Infrastructure.Outbox;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", "outbox");
        builder.HasIndex(x => x.Id).IsUnique();
        //Ensure Content is JSON
        builder.ToTable(x => x.HasCheckConstraint("CK_Outbox_Content_Json", "\"Content\"::jsonb IS NOT NULL AND \"Error\"::jsonb IS NOT NULL"));

        builder.Property(x => x.Type).IsRequired();
        builder.HasIndex(x => x.ProcessedOnUtc).HasMethod("btree");
        builder.HasIndex(x => x.Attempt).IsUnique(false);
    }
}