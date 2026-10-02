using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.TimeTracking.Dtos;
using ProjectTracker.Application.Features.TimeTracking.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.TimeTracking.Queries.GetTimeEntries;

public record GetTimeEntriesQuery(Guid TaskId, int Page, int PageSize);

public interface IGetTimeEntriesHandler
{
    Task<PagedResult<TimeEntryDto>> Handle(GetTimeEntriesQuery query, CancellationToken ct);
}

public class GetTimeEntriesHandler : IGetTimeEntriesHandler
{
    private readonly IApplicationDbContext _db;

    public GetTimeEntriesHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<TimeEntryDto>> Handle(GetTimeEntriesQuery query, CancellationToken ct)
    {
        var q = _db.TimeEntries.AsNoTracking()
            .Where(e => e.TaskId == query.TaskId)
            .OrderByDescending(e => e.Date).ThenByDescending(e => e.CreatedAt);

        var totalCount = await q.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(TimeEntryProjection.ToDto).ToListAsync(ct);

        return new PagedResult<TimeEntryDto>(items, page, pageSize, totalCount);
    }
}
