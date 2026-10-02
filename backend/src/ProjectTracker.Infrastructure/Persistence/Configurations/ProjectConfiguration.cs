using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Key).HasMaxLength(10).IsRequired();
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.Priority).HasConversion<string>().HasMaxLength(20);
        builder.Property(p => p.RowVersion).IsRowVersion();

        builder.HasIndex(p => new { p.OrganizationId, p.Key }).IsUnique();
        builder.HasIndex(p => p.OrganizationId);
        builder.HasIndex(p => p.Status);

        builder.HasOne(p => p.Organization).WithMany().HasForeignKey(p => p.OrganizationId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(p => p.Owner).WithMany().HasForeignKey(p => p.OwnerUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.Manager).WithMany().HasForeignKey(p => p.ManagerUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("ProjectMembers");
        builder.HasKey(pm => pm.Id);
        builder.Property(pm => pm.RowVersion).IsRowVersion();
        builder.HasIndex(pm => new { pm.ProjectId, pm.UserId }).IsUnique();

        builder.HasOne(pm => pm.Project).WithMany(p => p.Members).HasForeignKey(pm => pm.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pm => pm.User).WithMany().HasForeignKey(pm => pm.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pm => pm.Role).WithMany().HasForeignKey(pm => pm.RoleId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectTeamConfiguration : IEntityTypeConfiguration<ProjectTeam>
{
    public void Configure(EntityTypeBuilder<ProjectTeam> builder)
    {
        builder.ToTable("ProjectTeams");
        builder.HasKey(pt => new { pt.ProjectId, pt.TeamId });

        builder.HasOne(pt => pt.Project).WithMany(p => p.ProjectTeams).HasForeignKey(pt => pt.ProjectId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(pt => pt.Team).WithMany(t => t.ProjectTeams).HasForeignKey(pt => pt.TeamId).OnDelete(DeleteBehavior.Restrict);
    }
}
