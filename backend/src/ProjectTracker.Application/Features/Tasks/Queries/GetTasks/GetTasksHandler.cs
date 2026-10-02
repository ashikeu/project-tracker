using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Application.Features.Tasks.Mappings;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Queries.GetTasks;

public interface IGetTasksHandler
{
    Task<PagedResult<TaskDto>> Handle(GetTasksQuery query, CancellationToken ct);
}

/// <summary>Backs GET /tasks. Every filter in planning.md §25 is optional and AND-combined;
/// results are always paginated (see CLAUDE.md "No unbounded GET endpoints").</summary>
public class GetTasksHandler : IGetTasksHandler
{
    private readonly IApplicationDbContext _db;

    public GetTasksHandler(IApplicationDbContext db) => _db = db;

    public async Task<PagedResult<TaskDto>> Handle(GetTasksQuery query, CancellationToken ct)
    {
        IQueryable<TaskItem> q = _db.Tasks.AsNoTracking();

        if (query.ProjectId is not null) q = q.Where(t => t.ProjectId == query.ProjectId);
        if (query.EpicId is not null) q = q.Where(t => t.EpicId == query.EpicId);
        if (query.SprintId is not null) q = q.Where(t => t.SprintId == query.SprintId);
        if (query.Status is not null) q = q.Where(t => t.Status == query.Status);
        if (query.Priority is not null) q = q.Where(t => t.Priority == query.Priority);
        if (query.TaskType is not null) q = q.Where(t => t.TaskType == query.TaskType);
        if (query.AssigneeUserId is not null) q = q.Where(t => t.AssigneeUserId == query.AssigneeUserId);
        if (query.ReporterUserId is not null) q = q.Where(t => t.ReporterUserId == query.ReporterUserId);
        if (query.TeamId is not null) q = q.Where(t => t.TeamId == query.TeamId);
        if (query.LabelId is not null) q = q.Where(t => t.TaskLabels.Any(tl => tl.LabelId == query.LabelId));
        if (query.DueBefore is not null) q = q.Where(t => t.DueDate != null && t.DueDate <= query.DueBefore);
        if (query.DueAfter is not null) q = q.Where(t => t.DueDate != null && t.DueDate >= query.DueAfter);
        if (query.UnassignedOnly) q = q.Where(t => t.AssigneeUserId == null);
        if (query.TopLevelOnly) q = q.Where(t => t.ParentTaskId == null);
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(t => t.Title.Contains(term) || t.TaskKey.Contains(term));
        }

        q = (query.SortBy?.ToLowerInvariant()) switch
        {
            "priority" => query.SortDescending ? q.OrderByDescending(t => t.Priority) : q.OrderBy(t => t.Priority),
            "duedate" => query.SortDescending ? q.OrderByDescending(t => t.DueDate) : q.OrderBy(t => t.DueDate),
            "updatedat" => query.SortDescending ? q.OrderByDescending(t => t.UpdatedAt) : q.OrderBy(t => t.UpdatedAt),
            "position" => query.SortDescending ? q.OrderByDescending(t => t.Position) : q.OrderBy(t => t.Position),
            _ => query.SortDescending ? q.OrderByDescending(t => t.CreatedAt) : q.OrderBy(t => t.CreatedAt)
        };

        var totalCount = await q.CountAsync(ct);
        var page = Math.Max(query.Page, 1);
        var pageSize = query.PageSize <= 0 ? 50 : Math.Min(query.PageSize, 200);

        var items = await q.Skip((page - 1) * pageSize).Take(pageSize).Select(TaskMappings.ToDto).ToListAsync(ct);

        return new PagedResult<TaskDto>(items, page, pageSize, totalCount);
    }
}
