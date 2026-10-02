using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>An in-app notification addressed to a single user. Isolation is by UserId (a user only
/// ever queries their own), so this is intentionally NOT an ITenantEntity — handlers always filter
/// by the authenticated UserId, and there is no cross-user read path.</summary>
public class Notification : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    /// <summary>Deep-link target, e.g. "Task" + the task id, so the client can navigate on tap.</summary>
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
}
