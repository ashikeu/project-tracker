namespace ProjectTracker.Application.Common.Interfaces;

/// <summary>Writes append-only ActivityLog rows. The organization and acting user are taken from
/// the current request (ICurrentUserService) by the implementation, so callers pass only what
/// changed. Logging is best-effort relative to the mutation — call it after the mutation has been
/// persisted.</summary>
public interface IActivityLogger
{
    Task LogAsync(string entityType, Guid entityId, string action, object? oldValue, object? newValue, CancellationToken ct);
}
