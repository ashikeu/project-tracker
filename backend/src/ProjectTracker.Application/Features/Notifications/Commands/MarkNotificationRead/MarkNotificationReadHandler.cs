using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Notifications.Commands.MarkNotificationRead;

public record MarkNotificationReadCommand(Guid NotificationId);

public interface IMarkNotificationReadHandler
{
    Task<Result> Handle(MarkNotificationReadCommand command, CancellationToken ct);
}

public class MarkNotificationReadHandler : IMarkNotificationReadHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationReadHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(MarkNotificationReadCommand command, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == command.NotificationId && n.UserId == userId, ct);
        if (notification is null) return Result.Fail("Notification not found.");

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
        return Result.Ok("Notification marked as read.");
    }
}
