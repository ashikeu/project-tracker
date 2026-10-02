using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

/// <summary>Per-user, per-type delivery switches. Absence of a row means "default on" for both
/// channels (see NotificationDispatcher) — a user only gets a row once they change a default.
/// Isolation is by UserId, like Notification.</summary>
public class NotificationPreference : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public NotificationType NotificationType { get; set; }
    public bool InAppEnabled { get; set; } = true;
    public bool PushEnabled { get; set; } = true;
}
