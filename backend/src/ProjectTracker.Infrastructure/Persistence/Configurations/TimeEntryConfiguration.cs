using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasIndex(e => e.TaskId);
        builder.HasIndex(e => e.OrganizationId);
        builder.HasIndex(e => e.UserId);

        builder.HasOne(e => e.Task).WithMany(t => t.TimeEntries).HasForeignKey(e => e.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Organization>().WithMany().HasForeignKey(e => e.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
