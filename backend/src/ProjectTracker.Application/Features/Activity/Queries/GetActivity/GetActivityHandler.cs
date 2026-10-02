using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Notifications.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Activity.Queries.GetActivity;

public record GetActivityQuery(string EntityType, Guid EntityId, int Page, int PageSize);

public interface IGetActivityHandler
{
    Task<PagedResult<ActivityLogDto>> Handle(GetActivityQuery query, CancellationToken ct);
}

/// <summary>Activity feed for one entity, newest first. Tenant-scoped automatically (ActivityLog is
/// an ITenantEntity, so the global query filter restricts it to the caller's organization).</summary>
public class GetActivityHandler : IGetActivityHandler
{
    private readonly IApplicationDbContext _db;

    public GetActivityHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<ActivityLogDto>> Handle(GetActivityQuery query, CancellationToken ct)
    {
        var q = _db.ActivityLogs.AsNoTracking()
            .Where(a => a.EntityType == query.EntityType && a.EntityId == query.EntityId)
            .OrderByDescending(a => a.CreatedAt);

        var totalCount = await q.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(a => new ActivityLogDto(
                a.Id, a.EntityType, a.EntityId, a.UserId,
                a.User != null ? a.User.FirstName + " " + a.User.LastName : null,
                a.Action, a.OldValueJson, a.NewValueJson, a.CreatedAt))
            .ToListAsync(ct);

        return new PagedResult<ActivityLogDto>(items, page, pageSize, totalCount);
    }
}
