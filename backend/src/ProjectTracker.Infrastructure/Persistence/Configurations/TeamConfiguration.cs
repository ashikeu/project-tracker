using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(150).IsRequired();
        builder.Property(t => t.RowVersion).IsRowVersion();
        builder.HasIndex(t => t.OrganizationId);
        builder.HasIndex(t => new { t.OrganizationId, t.Name });

        builder.HasOne(t => t.Organization).WithMany().HasForeignKey(t => t.OrganizationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(t => t.Lead).WithMany().HasForeignKey(t => t.LeadUserId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.ToTable("TeamMembers");
        builder.HasKey(tm => tm.Id);
        builder.Property(tm => tm.Role).HasConversion<string>().HasMaxLength(20);
        builder.Property(tm => tm.RowVersion).IsRowVersion();
        builder.HasIndex(tm => new { tm.TeamId, tm.UserId }).IsUnique();

        builder.HasOne(tm => tm.Team).WithMany(t => t.Members).HasForeignKey(tm => tm.TeamId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(tm => tm.User).WithMany().HasForeignKey(tm => tm.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
