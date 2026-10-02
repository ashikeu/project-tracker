using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.TaskKey).HasMaxLength(20).IsRequired();
        builder.HasIndex(t => t.TaskKey).IsUnique();
        builder.Property(t => t.Title).HasMaxLength(500).IsRequired();
        builder.Property(t => t.TaskType).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.EstimatedHours).HasPrecision(8, 2);
        builder.Property(t => t.RemainingHours).HasPrecision(8, 2);
        builder.Property(t => t.LoggedHours).HasPrecision(8, 2);
        builder.Property(t => t.RowVersion).IsRowVersion();

        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.OrganizationId);
        builder.HasIndex(t => t.EpicId);
        builder.HasIndex(t => t.SprintId);
        builder.HasIndex(t => t.AssigneeUserId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
        builder.HasIndex(t => t.ParentTaskId);
        builder.HasIndex(t => t.CreatedAt);
        builder.HasIndex(t => t.UpdatedAt);
        builder.HasIndex(t => t.BoardColumnId);

        builder.HasOne(t => t.Project).WithMany().HasForeignKey(t => t.ProjectId).OnDelete(DeleteBehavior.Cascade);
        // Restrict, not SetNull: SQL Server counts SET NULL as a cascading action too, and
        // Project already cascades to Task directly — a second cascading path via Epic (also
        // reachable from Project) triggers SQL Server's "multiple cascade paths" error. The app
        // enforces "can't delete an epic that still has tasks" instead (see DeleteEpicHandler).
        builder.HasOne(t => t.Epic).WithMany(e => e.Tasks).HasForeignKey(t => t.EpicId).OnDelete(DeleteBehavior.Restrict);
        // Restrict for the same "multiple cascade paths via Project" reason as EpicId — Project
        // cascades to both Sprint and Task, so a cascading Sprint→Task path would collide. The app
        // reassigns tasks off a sprint (or blocks the delete) rather than DB-cascading.
        builder.HasOne(t => t.Sprint).WithMany(s => s.Tasks).HasForeignKey(t => t.SprintId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.ParentTask).WithMany(t => t.Subtasks).HasForeignKey(t => t.ParentTaskId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Assignee).WithMany().HasForeignKey(t => t.AssigneeUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Reporter).WithMany().HasForeignKey(t => t.ReporterUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(t => t.Team).WithMany().HasForeignKey(t => t.TeamId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Organization>().WithMany().HasForeignKey(t => t.OrganizationId).OnDelete(DeleteBehavior.Restrict);
        // Restrict for the same "multiple cascade paths via Project" reason as EpicId above.
        builder.HasOne(t => t.BoardColumn).WithMany().HasForeignKey(t => t.BoardColumnId).OnDelete(DeleteBehavior.Restrict);
    }
}
