namespace ProjectTracker.Domain.Enums;

public enum ProjectStatus
{
    Planning = 0,
    Active = 1,
    OnHold = 2,
    Completed = 3,
    Archived = 4
}

/// <summary>Shared priority scale — used by Project, Epic and Task (see planning.md §10).</summary>
public enum Priority
{
    Lowest = 0,
    Low = 1,
    Medium = 2,
    High = 3,
    Highest = 4,
    Critical = 5
}
