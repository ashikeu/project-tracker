using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Reports.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Reports.Queries.GetCompletionReport;

public record GetCompletionReportQuery(Guid ProjectId, DateOnly? From, DateOnly? To);

public interface IGetCompletionReportHandler
{
    Task<Result<CompletionReportDto>> Handle(GetCompletionReportQuery query, CancellationToken ct);
}

/// <summary>Created-vs-completed trend for a project over a window (default last 30 days). Created is
/// from each task's CreatedAt; completed is the date a task first reached Done within the window
/// (reconstructed from ActivityLog). Top-level tasks only.</summary>
public class GetCompletionReportHandler : IGetCompletionReportHandler
{
    private readonly IApplicationDbContext _db;

    public GetCompletionReportHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<CompletionReportDto>> Handle(GetCompletionReportQuery query, CancellationToken ct)
    {
        var projectExists = await _db.Projects.AsNoTracking().AnyAsync(p => p.Id == query.ProjectId, ct);
        if (!projectExists) return Result<CompletionReportDto>.Fail("Project not found.");

        var to = query.To ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var from = query.From ?? to.AddDays(-30);
        var fromDt = from.ToDateTime(TimeOnly.MinValue);
        var toDt = to.ToDateTime(TimeOnly.MaxValue);

        var createdByDate = await _db.Tasks.AsNoTracking()
            .Where(t => t.ProjectId == query.ProjectId && t.ParentTaskId == null
                        && t.CreatedAt >= fromDt && t.CreatedAt <= toDt)
            .Select(t => t.CreatedAt)
            .ToListAsync(ct);
        var createdCounts = createdByDate
            .GroupBy(dt => DateOnly.FromDateTime(dt))
            .ToDictionary(g => g.Key, g => g.Count());

        var projectTaskIds = await _db.Tasks.AsNoTracking()
            .Where(t => t.ProjectId == query.ProjectId && t.ParentTaskId == null)
            .Select(t => t.Id).ToListAsync(ct);

        var doneLogs = await _db.ActivityLogs.AsNoTracking()
            .Where(a => a.EntityType == "Task" && projectTaskIds.Contains(a.EntityId)
                        && a.NewValueJson != null && a.NewValueJson.Contains("\"Status\":\"Done\"")
                        && a.CreatedAt >= fromDt && a.CreatedAt <= toDt)
            .Select(a => new { a.EntityId, a.CreatedAt })
            .ToListAsync(ct);
        // First time each task reached Done within the window.
        var completedCounts = doneLogs
            .GroupBy(l => l.EntityId)
            .Select(g => DateOnly.FromDateTime(g.Min(x => x.CreatedAt)))
            .GroupBy(d => d)
            .ToDictionary(g => g.Key, g => g.Count());

        var points = new List<CompletionTrendPoint>();
        for (var d = from; d <= to; d = d.AddDays(1))
        {
            points.Add(new CompletionTrendPoint(d,
                createdCounts.GetValueOrDefault(d, 0),
                completedCounts.GetValueOrDefault(d, 0)));
        }

        return Result<CompletionReportDto>.Ok(new CompletionReportDto(
            query.ProjectId, from, to, points.Sum(p => p.Created), points.Sum(p => p.Completed), points));
    }
}
