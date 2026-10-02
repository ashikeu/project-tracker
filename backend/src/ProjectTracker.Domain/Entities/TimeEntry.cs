using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

/// <summary>A logged block of work on a Task — either entered manually (DurationMinutes set up
/// front) or produced by the start/stop timer (IsRunningTimer + StartedAt, DurationMinutes filled
/// in on stop). The owning Task's LoggedHours/RemainingHours are recomputed from the sum of its
/// entries whenever one changes, so they never drift.</summary>
public class TimeEntry : BaseAuditableEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }

    public Guid TaskId { get; set; }
    public TaskItem Task { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public DateOnly Date { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsBillable { get; set; }
    public string? Description { get; set; }

    /// <summary>True while a timer is running for this entry; DurationMinutes is finalized on stop.</summary>
    public bool IsRunningTimer { get; set; }
    public DateTime? StartedAt { get; set; }
}
