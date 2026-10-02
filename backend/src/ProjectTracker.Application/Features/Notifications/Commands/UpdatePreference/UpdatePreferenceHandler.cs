using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Notifications.Dtos;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Notifications.Commands.UpdatePreference;

public record UpdatePreferenceCommand(NotificationType NotificationType, bool InAppEnabled, bool PushEnabled);

public interface IUpdatePreferenceHandler
{
    Task<Result<NotificationPreferenceDto>> Handle(UpdatePreferenceCommand command, CancellationToken ct);
}

/// <summary>Upserts the caller's preference row for one notification type.</summary>
public class UpdatePreferenceHandler : IUpdatePreferenceHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdatePreferenceHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<NotificationPreferenceDto>> Handle(UpdatePreferenceCommand command, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;
        var pref = await _db.NotificationPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId && p.NotificationType == command.NotificationType, ct);

        if (pref is null)
        {
            pref = new NotificationPreference
            {
                UserId = userId,
                NotificationType = command.NotificationType,
                InAppEnabled = command.InAppEnabled,
                PushEnabled = command.PushEnabled
            };
            _db.NotificationPreferences.Add(pref);
        }
        else
        {
            pref.InAppEnabled = command.InAppEnabled;
            pref.PushEnabled = command.PushEnabled;
        }

        await _db.SaveChangesAsync(ct);
        return Result<NotificationPreferenceDto>.Ok(new NotificationPreferenceDto(
            pref.NotificationType.ToString(), pref.InAppEnabled, pref.PushEnabled));
    }
}
