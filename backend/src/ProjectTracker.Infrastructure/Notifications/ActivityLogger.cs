using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Notifications;

/// <summary>Writes ActivityLog rows scoped to the current request's organization/user. Append-only:
/// it only ever inserts. A missing OrganizationId (e.g. outside a request) means there's nothing to
/// scope the log to, so the call is skipped rather than throwing — activity logging must never be
/// the thing that fails a mutation.</summary>
public class ActivityLogger : IActivityLogger
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<ActivityLogger> _logger;

    public ActivityLogger(IApplicationDbContext db, ICurrentUserService currentUser, ILogger<ActivityLogger> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task LogAsync(string entityType, Guid entityId, string action, object? oldValue, object? newValue, CancellationToken ct)
    {
        var orgId = _currentUser.OrganizationId;
        if (orgId is null)
        {
            _logger.LogDebug("Skipping activity log for {EntityType} {EntityId} — no organization in scope.", entityType, entityId);
            return;
        }

        _db.ActivityLogs.Add(new ActivityLog
        {
            OrganizationId = orgId.Value,
            EntityType = entityType,
            EntityId = entityId,
            UserId = _currentUser.UserId,
            Action = action,
            OldValueJson = oldValue is null ? null : JsonSerializer.Serialize(oldValue),
            NewValueJson = newValue is null ? null : JsonSerializer.Serialize(newValue),
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);
    }
}
