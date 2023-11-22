using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sharecode.Backend.Domain.Entity.Gateway;

namespace Sharecode.Backend.Infrastructure.Configuration;

public class GatewayConfiguration : IEntityTypeConfiguration<GatewayRequest>
{
    public void Configure(EntityTypeBuilder<GatewayRequest> builder)
    {
        builder.HasIndex(p => p.SourceId);
        
        builder.HasIndex(p => new { p.SourceId, p.RequestType, p.IsValid, p.IsDeleted, p.Expiry, p.ProcessedAt,
            Completed = p.IsCompleted });
        
        // Partial index for IsDeleted
        builder.HasIndex(p => p.IsDeleted)
            .HasFilter("IsDeleted = 0");

        // Partial index for Completed
        builder.HasIndex(p => p.IsCompleted)
            .HasFilter("IsCompleted = 0");
    }
} 