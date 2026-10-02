namespace ProjectTracker.Domain.Enums;

public enum EpicStatus
{
    Planning = 0,
    Active = 1,
    Completed = 2,
    Archived = 3
}

/// <summary>Canonical workflow state. Project-level BoardColumns (Phase 5) provide the
/// customizable display name/order/color/WIP-limit per column, but every column maps to one of
/// these fixed states so cross-project reporting, filtering and the burndown chart stay
/// well-defined regardless of a project's custom column names.
/// Named TaskItemStatus (not TaskStatus) — System.Threading.Tasks.TaskStatus is implicitly in
/// scope everywhere via ImplicitUsings and would collide.</summary>
public enum TaskItemStatus
{
    Backlog = 0,
    Todo = 1,
    InProgress = 2,
    InReview = 3,
    Testing = 4,
    Done = 5
}

public enum TaskType
{
    Task = 0,
    Bug = 1,
    Story = 2,
    Feature = 3,
    Improvement = 4,
    Incident = 5,
    Request = 6
}

public enum TaskDependencyType
{
    Blocks = 0,
    BlockedBy = 1,
    RelatesTo = 2,
    Duplicates = 3,
    DependsOn = 4
}
