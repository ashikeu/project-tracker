using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Reports.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Reports.Queries.GetWorkloadReport;

public record GetWorkloadReportQuery(Guid ProjectId);

public interface IGetWorkloadReportHandler
{
    Task<Result<WorkloadReportDto>> Handle(GetWorkloadReportQuery query, CancellationToken ct);
}

/// <summary>Per-assignee open workload for a project: open task count, open story points, overdue
/// count, and total hours logged. Only top-level tasks count toward workload.</summary>
public class GetWorkloadReportHandler : IGetWorkloadReportHandler
{
    private readonly IApplicationDbContext _db;

    public GetWorkloadReportHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<WorkloadReportDto>> Handle(GetWorkloadReportQuery query, CancellationToken ct)
    {
        var projectExists = await _db.Projects.AsNoTracking().AnyAsync(p => p.Id == query.ProjectId, ct);
        if (!projectExists) return Result<WorkloadReportDto>.Fail("Project not found.");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var openByAssignee = await _db.Tasks.AsNoTracking()
            .Where(t => t.ProjectId == query.ProjectId && t.ParentTaskId == null
                        && t.AssigneeUserId != null && t.Status != TaskItemStatus.Done)
            .GroupBy(t => new { t.AssigneeUserId, Name = t.Assignee!.FirstName + " " + t.Assignee.LastName })
            .Select(g => new
            {
                g.Key.AssigneeUserId,
                g.Key.Name,
                OpenTasks = g.Count(),
                OpenStoryPoints = g.Sum(t => t.StoryPoints ?? 0),
                OverdueTasks = g.Count(t => t.DueDate != null && t.DueDate < today)
            })
            .ToListAsync(ct);

        // Hours logged per user on this project's tasks.
        var loggedByUser = await _db.TimeEntries.AsNoTracking()
            .Where(e => e.Task.ProjectId == query.ProjectId && !e.IsRunningTimer)
            .GroupBy(e => e.UserId)
            .Select(g => new { UserId = g.Key, Minutes = g.Sum(e => e.DurationMinutes) })
            .ToDictionaryAsync(x => x.UserId, x => x.Minutes, ct);

        var rows = openByAssignee.Select(r => new WorkloadReportRow(
            r.AssigneeUserId, r.Name, r.OpenTasks, r.OpenStoryPoints, r.OverdueTasks,
            r.AssigneeUserId is not null && loggedByUser.TryGetValue(r.AssigneeUserId.Value, out var m)
                ? Math.Round(m / 60m, 2) : 0m))
            .OrderByDescending(r => r.OpenTasks)
            .ToList();

        return Result<WorkloadReportDto>.Ok(new WorkloadReportDto(query.ProjectId, rows));
    }
}
