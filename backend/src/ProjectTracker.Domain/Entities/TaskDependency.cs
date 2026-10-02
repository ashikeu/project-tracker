using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

public class TaskDependency : BaseEntity
{
    public Guid TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;

    public Guid DependsOnTaskId { get; set; }
    public TaskItem DependsOnTask { get; set; } = null!;

    public TaskDependencyType Type { get; set; }
}
