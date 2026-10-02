using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

/// <summary>A user's membership on a project, with the Role that governs their permissions in
/// the project's context (may differ from their organization-level role — e.g. an
/// Organization Admin might sit on a project as a plain Developer).</summary>
public class ProjectMember : BaseAuditableEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

public class ProjectTeam
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public Guid TeamId { get; set; }
    public Team Team { get; set; } = null!;
}
