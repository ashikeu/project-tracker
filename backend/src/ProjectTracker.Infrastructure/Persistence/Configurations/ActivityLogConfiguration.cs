using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.EntityType).HasMaxLength(40).IsRequired();
        builder.Property(a => a.Action).HasMaxLength(60).IsRequired();
        builder.HasIndex(a => a.OrganizationId);
        // "Activity for this entity, newest first" (e.g. GET /tasks/{id}/activity).
        builder.HasIndex(a => new { a.EntityType, a.EntityId, a.CreatedAt });

        builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Organization>().WithMany().HasForeignKey(a => a.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
