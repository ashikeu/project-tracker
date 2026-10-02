namespace ProjectTracker.Domain.Enums;

/// <summary>Kinds of in-app/push notification. Also the key NotificationPreferences are keyed by,
/// so a user can mute a category independently for in-app vs push.</summary>
public enum NotificationType
{
    TaskAssigned = 0,
    TaskStatusChanged = 1,
    CommentMention = 2,
    CommentAdded = 3,
    TaskDueSoon = 4
}
