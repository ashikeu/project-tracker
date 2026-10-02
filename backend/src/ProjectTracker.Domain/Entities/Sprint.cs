using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>A time-boxed iteration owned by a Project. Tasks are pulled from the backlog into a
/// Sprint via TaskItem.SprintId. Status follows a guarded state machine — see
/// Domain/BusinessRules/SprintStatusTransition.</summary>
public class Sprint : BaseSoftDeleteEntity, ITenantEntity
{
    /// <summary>Denormalized from Project.OrganizationId at creation time — see Epic.OrganizationId
    /// for the rationale (keeps the tenant-isolation global query filter direct, not join-dependent).</summary>
    public Guid OrganizationId { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }

    public SprintStatus Status { get; set; } = SprintStatus.Planning;

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    /// <summary>Set when the sprint transitions to Completed.</summary>
    public DateTime? CompletedAt { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
