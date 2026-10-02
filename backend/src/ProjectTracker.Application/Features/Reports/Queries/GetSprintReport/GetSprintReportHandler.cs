using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Reports.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Reports.Queries.GetSprintReport;

public record GetSprintReportQuery(Guid SprintId);

public interface IGetSprintReportHandler
{
    Task<Result<SprintReportDto>> Handle(GetSprintReportQuery query, CancellationToken ct);
}

public class GetSprintReportHandler : IGetSprintReportHandler
{
    private readonly IApplicationDbContext _db;

    public GetSprintReportHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<SprintReportDto>> Handle(GetSprintReportQuery query, CancellationToken ct)
    {
        var sprint = await _db.Sprints.AsNoTracking()
            .Where(s => s.Id == query.SprintId)
            .Select(s => new { s.Id, s.Name, s.Status, s.StartDate, s.EndDate, s.CompletedAt })
            .FirstOrDefaultAsync(ct);
        if (sprint is null) return Result<SprintReportDto>.Fail("Sprint not found.");

        var tasks = _db.Tasks.AsNoTracking().Where(t => t.SprintId == query.SprintId);

        var total = await tasks.CountAsync(ct);
        var completed = await tasks.CountAsync(t => t.Status == TaskItemStatus.Done, ct);
        var committedPoints = await tasks.SumAsync(t => (int?)t.StoryPoints ?? 0, ct);
        var completedPoints = await tasks.Where(t => t.Status == TaskItemStatus.Done).SumAsync(t => (int?)t.StoryPoints ?? 0, ct);
        var byStatus = await tasks.GroupBy(t => t.Status)
            .Select(g => new StatusCount(g.Key.ToString(), g.Count())).ToListAsync(ct);

        var completionPercent = total == 0 ? 0 : Math.Round(completed * 100.0 / total, 1);

        return Result<SprintReportDto>.Ok(new SprintReportDto(
            sprint.Id, sprint.Name, sprint.Status.ToString(), sprint.StartDate, sprint.EndDate, sprint.CompletedAt,
            total, completed, committedPoints, completedPoints, completionPercent, byStatus));
    }
}
