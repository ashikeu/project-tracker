using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Persistence.Configurations;

public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("Labels");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).HasMaxLength(50).IsRequired();
        builder.Property(l => l.Color).HasMaxLength(20).IsRequired();
        builder.Property(l => l.RowVersion).IsRowVersion();
        builder.HasIndex(l => new { l.ProjectId, l.Name }).IsUnique();

        builder.HasOne(l => l.Project).WithMany().HasForeignKey(l => l.ProjectId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class TaskLabelConfiguration : IEntityTypeConfiguration<TaskLabel>
{
    public void Configure(EntityTypeBuilder<TaskLabel> builder)
    {
        builder.ToTable("TaskLabels");
        builder.HasKey(tl => new { tl.TaskId, tl.LabelId });

        builder.HasOne(tl => tl.Task).WithMany(t => t.TaskLabels).HasForeignKey(tl => tl.TaskId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(tl => tl.Label).WithMany(l => l.TaskLabels).HasForeignKey(tl => tl.LabelId).OnDelete(DeleteBehavior.Restrict);
    }
}
