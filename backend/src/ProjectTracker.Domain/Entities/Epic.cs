using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

public class Epic : BaseSoftDeleteEntity, ITenantEntity
{
    /// <summary>Denormalized from Project.OrganizationId at creation time so the global
    /// tenant-isolation query filter (which only inspects ITenantEntity.OrganizationId) protects
    /// Epics directly, without relying on every query remembering to join through Project.</summary>
    public Guid OrganizationId { get; set; }

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid? OwnerUserId { get; set; }
    public User? Owner { get; set; }

    public EpicStatus Status { get; set; } = EpicStatus.Planning;
    public Priority Priority { get; set; } = Priority.Medium;

    public string? Color { get; set; }
    public string? Icon { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
