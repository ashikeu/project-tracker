using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

public class Label : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6B7280";

    public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
}

public class TaskLabel
{
    public Guid TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;

    public Guid LabelId { get; set; }
    public Label Label { get; set; } = null!;
}
