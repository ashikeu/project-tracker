using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Sync.Dtos;

namespace ProjectTracker.Application.Features.Sync.Queries.PullChanges;

public record PullChangesQuery(long Since, int PageSize);

public interface IPullChangesHandler
{
    Task<SyncPullResult> Handle(PullChangesQuery query, CancellationToken ct);
}

/// <summary>Delta pull: the caller's org changes with ServerVersion greater than the client's
/// watermark, in order, paged. HasMore tells the client to pull again from NextVersion until it
/// catches up. Tenant-scoped by the global query filter.</summary>
public class PullChangesHandler : IPullChangesHandler
{
    private const int MaxPageSize = 500;

    private readonly IApplicationDbContext _db;

    public PullChangesHandler(IApplicationDbContext db) => _db = db;

    public async Task<SyncPullResult> Handle(PullChangesQuery query, CancellationToken ct)
    {
        var pageSize = query.PageSize <= 0 ? 200 : Math.Min(query.PageSize, MaxPageSize);

        // Fetch one extra to detect whether more remain beyond this page.
        var rows = await _db.SyncChanges.AsNoTracking()
            .Where(c => c.ServerVersion > query.Since)
            .OrderBy(c => c.ServerVersion)
            .Take(pageSize + 1)
            .Select(c => new SyncChangeDto(c.ServerVersion, c.EntityType, c.EntityId, c.ChangeType.ToString(), c.PayloadJson))
            .ToListAsync(ct);

        var hasMore = rows.Count > pageSize;
        var changes = hasMore ? rows.Take(pageSize).ToList() : rows;
        var nextVersion = changes.Count > 0 ? changes[^1].ServerVersion : query.Since;

        return new SyncPullResult(changes, nextVersion, hasMore);
    }
}
