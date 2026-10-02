using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

public class Project : BaseSoftDeleteEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    /// <summary>Short unique-per-organization code, e.g. "ERP" — prefixes generated task keys
    /// ("ERP-101"). Immutable once tasks exist under the project (enforced in the handler, not
    /// the DB, since it's a business rule rather than a structural constraint).</summary>
    public string Key { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid OwnerUserId { get; set; }
    public User Owner { get; set; } = null!;

    public Guid? ManagerUserId { get; set; }
    public User? Manager { get; set; }

    public ProjectStatus Status { get; set; } = ProjectStatus.Planning;
    public Priority Priority { get; set; } = Priority.Medium;

    public DateOnly? StartDate { get; set; }
    public DateOnly? TargetDate { get; set; }

    /// <summary>Next integer to assign when generating a task key for this project
    /// (ERP-&lt;NextTaskNumber&gt;), incremented transactionally on task creation.</summary>
    public int NextTaskNumber { get; set; } = 1;

    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }

    public string? SettingsJson { get; set; }

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();
}
