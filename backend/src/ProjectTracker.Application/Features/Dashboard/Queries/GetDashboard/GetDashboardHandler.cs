using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Dashboard.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Dashboard.Queries.GetDashboard;

public record GetDashboardQuery(Guid ProjectId);

public interface IGetDashboardHandler
{
    Task<Result<DashboardDto>> Handle(GetDashboardQuery query, CancellationToken ct);
}

/// <summary>Project overview: task counts, status/priority/type distributions, per-assignee workload,
/// epic progress, and the active sprint. Only top-level tasks are counted (subtasks roll up under
/// their parent) so numbers match what the board/backlog show.</summary>
public class GetDashboardHandler : IGetDashboardHandler
{
    private readonly IApplicationDbContext _db;

    public GetDashboardHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<DashboardDto>> Handle(GetDashboardQuery query, CancellationToken ct)
    {
        var project = await _db.Projects.AsNoTracking()
            .Where(p => p.Id == query.ProjectId).Select(p => new { p.Id, p.Name }).FirstOrDefaultAsync(ct);
        if (project is null) return Result<DashboardDto>.Fail("Project not found.");

        var tasks = _db.Tasks.AsNoTracking().Where(t => t.ProjectId == query.ProjectId && t.ParentTaskId == null);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var total = await tasks.CountAsync(ct);
        var done = await tasks.CountAsync(t => t.Status == TaskItemStatus.Done, ct);
        var overdue = await tasks.CountAsync(t => t.DueDate != null && t.DueDate < today && t.Status != TaskItemStatus.Done, ct);

        var byStatus = await tasks.GroupBy(t => t.Status)
            .Select(g => new CountBucket(g.Key.ToString(), g.Count())).ToListAsync(ct);
        var byPriority = await tasks.GroupBy(t => t.Priority)
            .Select(g => new CountBucket(g.Key.ToString(), g.Count())).ToListAsync(ct);
        var byType = await tasks.GroupBy(t => t.TaskType)
            .Select(g => new CountBucket(g.Key.ToString(), g.Count())).ToListAsync(ct);

        var workload = await tasks
            .Where(t => t.Status != TaskItemStatus.Done)
            .GroupBy(t => new { t.AssigneeUserId, Name = t.Assignee != null ? t.Assignee.FirstName + " " + t.Assignee.LastName : null })
            .Select(g => new WorkloadRow(
                g.Key.AssigneeUserId,
                g.Key.Name ?? "Unassigned",
                g.Count(),
                g.Sum(t => t.StoryPoints ?? 0)))
            .ToListAsync(ct);

        var epicProgress = await _db.Epics.AsNoTracking()
            .Where(e => e.ProjectId == query.ProjectId)
            .Select(e => new EpicProgressRow(
                e.Id, e.Name, e.Tasks.Count, e.Tasks.Count(t => t.Status == TaskItemStatus.Done)))
            .ToListAsync(ct);

        var activeSprint = await _db.Sprints.AsNoTracking()
            .Where(s => s.ProjectId == query.ProjectId && s.Status == SprintStatus.Active)
            .Select(s => new ActiveSprintSummary(
                s.Id, s.Name, s.Tasks.Count, s.Tasks.Count(t => t.Status == TaskItemStatus.Done),
                s.Tasks.Sum(t => t.StoryPoints ?? 0),
                s.Tasks.Where(t => t.Status == TaskItemStatus.Done).Sum(t => t.StoryPoints ?? 0)))
            .FirstOrDefaultAsync(ct);

        return Result<DashboardDto>.Ok(new DashboardDto(
            project.Id, project.Name, total, total - done, done, overdue,
            byStatus, byPriority, byType, workload, epicProgress, activeSprint));
    }
}
