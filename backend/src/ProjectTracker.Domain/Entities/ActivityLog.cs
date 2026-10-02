using ProjectTracker.Domain.Common;

namespace ProjectTracker.Domain.Entities;

/// <summary>Append-only audit trail of significant mutations, scoped to an organization. Never
/// updated or deleted (see ActivityLogConfiguration / the write path in ActivityLogger). Old/New
/// value JSON capture just the changed fields, so an activity feed can render "Status: Todo →
/// Done" without reconstructing full snapshots.</summary>
public class ActivityLog : BaseEntity, ITenantEntity
{
    public Guid OrganizationId { get; set; }

    /// <summary>e.g. "Task", "Comment" — paired with EntityId to locate what changed.</summary>
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }

    /// <summary>Who performed the action; null for system-generated activity (e.g. the due-soon job).</summary>
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Verb-ish key, e.g. "Created", "StatusChanged", "Assigned".</summary>
    public string Action { get; set; } = string.Empty;

    public string? OldValueJson { get; set; }
    public string? NewValueJson { get; set; }

    public DateTime CreatedAt { get; set; }
}
