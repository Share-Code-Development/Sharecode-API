using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Infrastructure.Outbox;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages", "Outbox");
        builder.HasIndex(x => x.Id).IsUnique();
        //Ensure Content is JSON
        builder.ToTable(x => x.HasCheckConstraint("CK_Outbox_Content_Json", "ISJSON([Content]) > 0 AND ISJSON([Error]) > 0"));

        builder.Property(x => x.Type).IsRequired();
        builder.HasIndex(x => x.ProcessedOnUtc).IsClustered(false);
        builder.HasIndex(x => x.Attempt).IsUnique(false);
    }
}