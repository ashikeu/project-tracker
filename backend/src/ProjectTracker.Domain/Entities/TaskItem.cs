using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>The core work-item entity (task, bug, story, ...). Named TaskItem, not Task — see
/// CLAUDE.md "Naming" for why. A row with ParentTaskId set is a Subtask of that row; there is no
/// separate Subtask table (matches planning.md's ERD).</summary>
public class TaskItem : BaseSoftDeleteEntity, ITenantEntity, ISyncableEntity
{
    /// <summary>Denormalized from Project.OrganizationId — see Epic.OrganizationId for why.</summary>
    public Guid OrganizationId { get; set; }

    /// <summary>Human-facing identifier, e.g. "ERP-101" — assigned transactionally from
    /// Project.NextTaskNumber at creation time (see CreateTaskHandler).</summary>
    public string TaskKey { get; set; } = string.Empty;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid? EpicId { get; set; }
    public Epic? Epic { get; set; }

    /// <summary>Null while the task sits in the product backlog; set when pulled into a Sprint.</summary>
    public Guid? SprintId { get; set; }
    public Sprint? Sprint { get; set; }

    /// <summary>Set when this row is a Subtask of another TaskItem.</summary>
    public Guid? ParentTaskId { get; set; }
    public TaskItem? ParentTask { get; set; }
    public ICollection<TaskItem> Subtasks { get; set; } = new List<TaskItem>();

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public TaskType TaskType { get; set; } = TaskType.Task;
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Backlog;
    public Priority Priority { get; set; } = Priority.Medium;

    public Guid? AssigneeUserId { get; set; }
    public User? Assignee { get; set; }

    public Guid ReporterUserId { get; set; }
    public User Reporter { get; set; } = null!;

    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }

    /// <summary>The Kanban column this task currently sits in (Phase 5). Independent of Status:
    /// moving a task to a column sets both BoardColumnId and Status (via the column's
    /// StatusMapping) together — see MoveTaskHandler.</summary>
    public Guid? BoardColumnId { get; set; }
    public BoardColumn? BoardColumn { get; set; }

    public int? StoryPoints { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? RemainingHours { get; set; }
    public decimal LoggedHours { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? DueDate { get; set; }

    /// <summary>Fractional-index ordering key for backlog/board drag-and-drop (see planning.md
    /// §9 risks — avoids full-list reindexing on every reorder). Unique only within whatever
    /// scope currently orders by it (e.g. backlog for this project); Phase 5 adds BoardColumnId
    /// as an additional ordering scope for the Kanban board.</summary>
    public double Position { get; set; }

    /// <summary>Set only for tasks created while offline; lets the sync engine (Phase 10)
    /// recognize a client-originated row across retried pushes before TaskKey exists.</summary>
    public Guid? ClientGeneratedId { get; set; }

    public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
    public ICollection<TaskWatcher> Watchers { get; set; } = new List<TaskWatcher>();
    public ICollection<ChecklistItem> ChecklistItems { get; set; } = new List<ChecklistItem>();
    public ICollection<TaskDependency> Dependencies { get; set; } = new List<TaskDependency>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
