using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class SprintConfiguration : IEntityTypeConfiguration<Sprint>
{
    public void Configure(EntityTypeBuilder<Sprint> builder)
    {
        builder.ToTable("Sprints");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Goal).HasMaxLength(2000);
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.RowVersion).IsRowVersion();
        builder.HasIndex(s => s.ProjectId);
        builder.HasIndex(s => s.OrganizationId);
        builder.HasIndex(s => s.Status);

        builder.HasOne(s => s.Project).WithMany().HasForeignKey(s => s.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Organization>().WithMany().HasForeignKey(s => s.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
