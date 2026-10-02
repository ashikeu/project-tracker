using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class OrganizationUserConfiguration : IEntityTypeConfiguration<OrganizationUser>
{
    public void Configure(EntityTypeBuilder<OrganizationUser> builder)
    {
        builder.ToTable("OrganizationUsers");
        builder.HasKey(ou => ou.Id);
        builder.HasIndex(ou => new { ou.OrganizationId, ou.UserId }).IsUnique();
        builder.Property(ou => ou.Status).HasConversion<string>().HasMaxLength(30);
        builder.Property(ou => ou.RowVersion).IsRowVersion();

        builder.HasOne(ou => ou.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(ou => ou.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ou => ou.User)
            .WithMany(u => u.OrganizationMemberships)
            .HasForeignKey(ou => ou.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ou => ou.Role)
            .WithMany()
            .HasForeignKey(ou => ou.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ou => ou.OrganizationId);
    }
}
