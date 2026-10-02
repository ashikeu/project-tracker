using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Infrastructure.Notifications;

/// <summary>Fans a notification out to recipients, honoring per-user, per-type preferences. Missing
/// preference rows mean "on" for both channels (opt-out model). The acting user is never notified
/// about their own action. In-app rows are written in one SaveChanges; push is attempted per
/// recipient afterward and its failures are swallowed (a dead device token must not fail the
/// originating request).</summary>
public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IPushSender _push;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(IApplicationDbContext db, ICurrentUserService currentUser,
        IPushSender push, ILogger<NotificationDispatcher> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _push = push;
        _logger = logger;
    }

    public async Task DispatchAsync(
        IEnumerable<Guid> recipientUserIds, NotificationType type, string title, string body,
        string? relatedEntityType, Guid? relatedEntityId, CancellationToken ct)
    {
        var actingUser = _currentUser.UserId;
        var recipients = recipientUserIds
            .Where(id => id != Guid.Empty && id != actingUser)
            .Distinct()
            .ToList();
        if (recipients.Count == 0) return;

        // One query for everyone's preference rows for this type; absence => default on.
        var prefs = await _db.NotificationPreferences.AsNoTracking()
            .Where(p => recipients.Contains(p.UserId) && p.NotificationType == type)
            .ToDictionaryAsync(p => p.UserId, ct);

        var pushRecipients = new List<Guid>();
        foreach (var userId in recipients)
        {
            prefs.TryGetValue(userId, out var pref);
            var inApp = pref?.InAppEnabled ?? true;
            var push = pref?.PushEnabled ?? true;

            if (inApp)
            {
                _db.Notifications.Add(new Notification
                {
                    UserId = userId,
                    Type = type,
                    Title = title,
                    Body = body,
                    RelatedEntityType = relatedEntityType,
                    RelatedEntityId = relatedEntityId
                });
            }
            if (push) pushRecipients.Add(userId);
        }

        await _db.SaveChangesAsync(ct);

        if (pushRecipients.Count == 0) return;

        var data = new Dictionary<string, string>
        {
            ["type"] = type.ToString(),
            ["entityType"] = relatedEntityType ?? string.Empty,
            ["entityId"] = relatedEntityId?.ToString() ?? string.Empty
        };
        foreach (var userId in pushRecipients)
        {
            try
            {
                await _push.SendAsync(userId, title, body, data, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Push notification to user {UserId} failed.", userId);
            }
        }
    }
}
