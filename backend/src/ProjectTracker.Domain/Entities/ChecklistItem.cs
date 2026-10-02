using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

public class ChecklistItem : BaseAuditableEntity
{
    public Guid TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;

    public string Text { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public int Order { get; set; }
}
