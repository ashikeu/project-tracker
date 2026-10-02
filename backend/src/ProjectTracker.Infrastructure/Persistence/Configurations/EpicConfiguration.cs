using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class EpicConfiguration : IEntityTypeConfiguration<Epic>
{
    public void Configure(EntityTypeBuilder<Epic> builder)
    {
        builder.ToTable("Epics");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Priority).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Color).HasMaxLength(20);
        builder.Property(e => e.Icon).HasMaxLength(50);
        builder.Property(e => e.RowVersion).IsRowVersion();
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.OrganizationId);

        builder.HasOne(e => e.Project).WithMany().HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Owner).WithMany().HasForeignKey(e => e.OwnerUserId).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<Organization>().WithMany().HasForeignKey(e => e.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
