using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Notifications.Commands.MarkAllRead;

public record MarkAllReadCommand;

public interface IMarkAllReadHandler
{
    Task<Result<int>> Handle(MarkAllReadCommand command, CancellationToken ct);
}

public class MarkAllReadHandler : IMarkAllReadHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkAllReadHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> Handle(MarkAllReadCommand command, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var unread = await _db.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToListAsync(ct);
        var now = DateTime.UtcNow;
        foreach (var n in unread)
        {
            n.IsRead = true;
            n.ReadAt = now;
        }
        if (unread.Count > 0) await _db.SaveChangesAsync(ct);
        return Result<int>.Ok(unread.Count, $"{unread.Count} notification(s) marked as read.");
    }
}
