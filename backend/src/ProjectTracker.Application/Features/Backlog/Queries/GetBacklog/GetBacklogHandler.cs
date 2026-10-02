using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Application.Features.Tasks.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Backlog.Queries.GetBacklog;

public interface IGetBacklogHandler
{
    Task<PagedResult<TaskDto>> Handle(GetBacklogQuery query, CancellationToken ct);
}

public class GetBacklogHandler : IGetBacklogHandler
{
    private readonly IApplicationDbContext _db;

    public GetBacklogHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<TaskDto>> Handle(GetBacklogQuery query, CancellationToken ct)
    {
        var q = _db.Tasks.AsNoTracking()
            .Where(t => t.ProjectId == query.ProjectId && t.SprintId == null && t.ParentTaskId == null)
            .OrderBy(t => t.Position);

        var totalCount = await q.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(TaskMappings.ToDto).ToListAsync(ct);

        return new PagedResult<TaskDto>(items, page, pageSize, totalCount);
    }
}
