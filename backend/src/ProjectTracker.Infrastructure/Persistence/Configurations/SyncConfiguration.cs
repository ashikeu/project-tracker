using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class SyncChangeConfiguration : IEntityTypeConfiguration<SyncChange>
{
    public void Configure(EntityTypeBuilder<SyncChange> builder)
    {
        builder.ToTable("SyncChanges");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.EntityType).HasMaxLength(40).IsRequired();
        builder.Property(c => c.ChangeType).HasConversion<string>().HasMaxLength(20);
        // Globally monotonic delta cursor — a bigint IDENTITY separate from the Guid PK.
        builder.Property(c => c.ServerVersion).ValueGeneratedOnAdd().UseIdentityColumn();
        builder.HasIndex(c => c.ServerVersion).IsUnique();
        // Delta-pull hot path: "my org's changes after cursor X, in order".
        builder.HasIndex(c => new { c.OrganizationId, c.ServerVersion });

        builder.HasOne<Organization>().WithMany().HasForeignKey(c => c.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class SyncOperationConfiguration : IEntityTypeConfiguration<SyncOperation>
{
    public void Configure(EntityTypeBuilder<SyncOperation> builder)
    {
        builder.ToTable("SyncOperations");
        // PK is the client-supplied OperationId — the idempotency key. Not DB-generated.
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever();
        builder.Property(o => o.EntityType).HasMaxLength(40).IsRequired();
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(o => o.OrganizationId);

        builder.HasOne<Organization>().WithMany().HasForeignKey(o => o.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
