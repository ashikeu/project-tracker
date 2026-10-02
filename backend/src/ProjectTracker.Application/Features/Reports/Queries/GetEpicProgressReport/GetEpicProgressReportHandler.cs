using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Reports.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Reports.Queries.GetEpicProgressReport;

public record GetEpicProgressReportQuery(Guid ProjectId);

public interface IGetEpicProgressReportHandler
{
    Task<Result<EpicProgressReportDto>> Handle(GetEpicProgressReportQuery query, CancellationToken ct);
}

public class GetEpicProgressReportHandler : IGetEpicProgressReportHandler
{
    private readonly IApplicationDbContext _db;

    public GetEpicProgressReportHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<EpicProgressReportDto>> Handle(GetEpicProgressReportQuery query, CancellationToken ct)
    {
        var projectExists = await _db.Projects.AsNoTracking().AnyAsync(p => p.Id == query.ProjectId, ct);
        if (!projectExists) return Result<EpicProgressReportDto>.Fail("Project not found.");

        var raw = await _db.Epics.AsNoTracking()
            .Where(e => e.ProjectId == query.ProjectId)
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.Status,
                TotalTasks = e.Tasks.Count,
                DoneTasks = e.Tasks.Count(t => t.Status == TaskItemStatus.Done),
                TotalPoints = e.Tasks.Sum(t => t.StoryPoints ?? 0),
                DonePoints = e.Tasks.Where(t => t.Status == TaskItemStatus.Done).Sum(t => t.StoryPoints ?? 0)
            })
            .ToListAsync(ct);

        var rows = raw.Select(e => new EpicProgressReportRow(
            e.Id, e.Name, e.Status.ToString(), e.TotalTasks, e.DoneTasks, e.TotalPoints, e.DonePoints,
            e.TotalTasks == 0 ? 0 : Math.Round(e.DoneTasks * 100.0 / e.TotalTasks, 1)))
            .ToList();

        return Result<EpicProgressReportDto>.Ok(new EpicProgressReportDto(query.ProjectId, rows));
    }
}
