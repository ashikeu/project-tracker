using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;

namespace ProjectTracker.Application.Features.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery;

public interface IGetUnreadCountHandler
{
    Task<int> Handle(GetUnreadCountQuery query, CancellationToken ct);
}

public class GetUnreadCountHandler : IGetUnreadCountHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadCountHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public Task<int> Handle(GetUnreadCountQuery query, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        return _db.Notifications.AsNoTracking().CountAsync(n => n.UserId == userId && !n.IsRead, ct);
    }
}
