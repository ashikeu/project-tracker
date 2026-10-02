using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Notifications.Dtos;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Notifications.Queries.GetPreferences;

public record GetPreferencesQuery;

public interface IGetPreferencesHandler
{
    Task<IReadOnlyList<NotificationPreferenceDto>> Handle(GetPreferencesQuery query, CancellationToken ct);
}

/// <summary>Returns a row for every NotificationType — the user's saved override where one exists,
/// otherwise the default (both channels on). So the client always gets the complete switch board.</summary>
public class GetPreferencesHandler : IGetPreferencesHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetPreferencesHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<NotificationPreferenceDto>> Handle(GetPreferencesQuery query, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var saved = await _db.NotificationPreferences.AsNoTracking()
            .Where(p => p.UserId == userId)
            .ToDictionaryAsync(p => p.NotificationType, ct);

        return Enum.GetValues<NotificationType>()
            .Select(type =>
            {
                saved.TryGetValue(type, out var pref);
                return new NotificationPreferenceDto(type.ToString(), pref?.InAppEnabled ?? true, pref?.PushEnabled ?? true);
            })
            .ToList();
    }
}
