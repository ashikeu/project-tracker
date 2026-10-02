using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Notifications.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Notifications.Queries.GetNotifications;

public record GetNotificationsQuery(bool UnreadOnly, int Page, int PageSize);

public interface IGetNotificationsHandler
{
    Task<PagedResult<NotificationDto>> Handle(GetNotificationsQuery query, CancellationToken ct);
}

/// <summary>Lists the current user's notifications, newest first. Scoped by the authenticated
/// UserId (not the tenant filter — Notification is per-user, not per-org).</summary>
public class GetNotificationsHandler : IGetNotificationsHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetNotificationsHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<NotificationDto>> Handle(GetNotificationsQuery query, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        var q = _db.Notifications.AsNoTracking().Where(n => n.UserId == userId);
        if (query.UnreadOnly) q = q.Where(n => !n.IsRead);
        q = q.OrderByDescending(n => n.CreatedAt);

        var totalCount = await q.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(n => new NotificationDto(
                n.Id, n.Type.ToString(), n.Title, n.Body, n.RelatedEntityType, n.RelatedEntityId,
                n.IsRead, n.ReadAt, n.CreatedAt))
            .ToListAsync(ct);

        return new PagedResult<NotificationDto>(items, page, pageSize, totalCount);
    }
}
