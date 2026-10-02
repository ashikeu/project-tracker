using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class TaskWatcherConfiguration : IEntityTypeConfiguration<TaskWatcher>
{
    public void Configure(EntityTypeBuilder<TaskWatcher> builder)
    {
        builder.ToTable("TaskWatchers");
        builder.HasKey(tw => new { tw.TaskId, tw.UserId });

        builder.HasOne(tw => tw.Task).WithMany(t => t.Watchers).HasForeignKey(tw => tw.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(tw => tw.User).WithMany().HasForeignKey(tw => tw.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.ToTable("ChecklistItems");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Text).HasMaxLength(500).IsRequired();
        builder.Property(c => c.RowVersion).IsRowVersion();
        builder.HasIndex(c => c.TaskId);

        builder.HasOne(c => c.Task).WithMany(t => t.ChecklistItems).HasForeignKey(c => c.TaskId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TaskDependencyConfiguration : IEntityTypeConfiguration<TaskDependency>
{
    public void Configure(EntityTypeBuilder<TaskDependency> builder)
    {
        builder.ToTable("TaskDependencies");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Type).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(d => new { d.TaskId, d.DependsOnTaskId, d.Type }).IsUnique();

        builder.HasOne(d => d.Task).WithMany(t => t.Dependencies).HasForeignKey(d => d.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(d => d.DependsOnTask).WithMany().HasForeignKey(d => d.DependsOnTaskId).OnDelete(DeleteBehavior.Restrict);
    }
}
