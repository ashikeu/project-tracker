using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>A customizable Kanban column for a project. Every column maps to one of the fixed
/// TaskItemStatus values (see that enum's doc comment) so cross-project reporting stays
/// well-defined even though projects can rename/reorder/recolor their columns freely.</summary>
public class BoardColumn : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#6B7280";
    public int Order { get; set; }
    public int? WipLimit { get; set; }
    public TaskItemStatus StatusMapping { get; set; }

    /// <summary>Tasks landing in this column are considered "done" for progress/burndown
    /// purposes — normally true for exactly one column (the last one).</summary>
    public bool IsCompletionColumn { get; set; }
}
