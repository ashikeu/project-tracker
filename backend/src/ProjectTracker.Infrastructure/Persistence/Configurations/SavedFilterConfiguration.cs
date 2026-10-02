using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class SavedFilterConfiguration : IEntityTypeConfiguration<SavedFilter>
{
    public void Configure(EntityTypeBuilder<SavedFilter> builder)
    {
        builder.ToTable("SavedFilters");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Name).HasMaxLength(150).IsRequired();
        builder.Property(f => f.Scope).HasMaxLength(40).IsRequired();
        builder.Property(f => f.FilterJson).IsRequired();
        builder.Property(f => f.RowVersion).IsRowVersion();
        builder.HasIndex(f => f.OrganizationId);
        builder.HasIndex(f => new { f.UserId, f.Scope });

        builder.HasOne(f => f.User).WithMany().HasForeignKey(f => f.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Organization>().WithMany().HasForeignKey(f => f.OrganizationId).OnDelete(DeleteBehavior.Restrict);
    }
}
