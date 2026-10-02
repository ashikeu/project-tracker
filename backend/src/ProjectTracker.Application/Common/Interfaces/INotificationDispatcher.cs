using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Common.Interfaces;

/// <summary>Creates notifications for recipients, honoring each recipient's per-type preferences
/// (in-app row written when InAppEnabled; push sent when PushEnabled). Deduplicates the acting user
/// out of the recipient set so people aren't notified about their own actions.</summary>
public interface INotificationDispatcher
{
    Task DispatchAsync(
        IEnumerable<Guid> recipientUserIds, NotificationType type, string title, string body,
        string? relatedEntityType, Guid? relatedEntityId, CancellationToken ct);
}
