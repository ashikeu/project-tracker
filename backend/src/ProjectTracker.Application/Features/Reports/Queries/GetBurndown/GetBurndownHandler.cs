using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Reports.Dtos;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Reports.Queries.GetBurndown;

public record GetBurndownQuery(Guid SprintId);

public interface IGetBurndownHandler
{
    Task<Result<BurndownDto>> Handle(GetBurndownQuery query, CancellationToken ct);
}

/// <summary>Sprint burndown: the ideal (linear) vs actual remaining story points across the sprint's
/// days. The actual line is reconstructed from ActivityLog — the date each task reached Done — so it
/// reflects real history rather than only the current snapshot. Days beyond today have no actual
/// point yet (null).</summary>
public class GetBurndownHandler : IGetBurndownHandler
{
    private readonly IApplicationDbContext _db;

    public GetBurndownHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<BurndownDto>> Handle(GetBurndownQuery query, CancellationToken ct)
    {
        var sprint = await _db.Sprints.AsNoTracking()
            .Where(s => s.Id == query.SprintId)
            .Select(s => new { s.Id, s.Name, s.StartDate, s.EndDate })
            .FirstOrDefaultAsync(ct);
        if (sprint is null) return Result<BurndownDto>.Fail("Sprint not found.");
        if (sprint.StartDate is null || sprint.EndDate is null)
        {
            return Result<BurndownDto>.Fail("Burndown requires the sprint to have both a start and end date.");
        }

        var start = sprint.StartDate.Value;
        var end = sprint.EndDate.Value;
        if (end < start) return Result<BurndownDto>.Fail("Sprint end date is before its start date.");

        var tasks = await _db.Tasks.AsNoTracking()
            .Where(t => t.SprintId == query.SprintId)
            .Select(t => new { t.Id, Points = t.StoryPoints ?? 0, t.Status, t.UpdatedAt, t.CreatedAt })
            .ToListAsync(ct);

        var total = tasks.Sum(t => t.Points);

        // Date each currently-Done task reached Done: latest activity whose new value recorded Done,
        // falling back to the row's last-updated timestamp when no such log exists.
        var doneTaskIds = tasks.Where(t => t.Status == TaskItemStatus.Done).Select(t => t.Id).ToList();
        var doneDates = new Dictionary<Guid, DateOnly>();
        if (doneTaskIds.Count > 0)
        {
            var logs = await _db.ActivityLogs.AsNoTracking()
                .Where(a => a.EntityType == "Task" && doneTaskIds.Contains(a.EntityId)
                            && a.NewValueJson != null && a.NewValueJson.Contains("\"Status\":\"Done\""))
                .Select(a => new { a.EntityId, a.CreatedAt })
                .ToListAsync(ct);

            foreach (var g in logs.GroupBy(l => l.EntityId))
            {
                doneDates[g.Key] = DateOnly.FromDateTime(g.Max(x => x.CreatedAt));
            }
            foreach (var t in tasks.Where(t => t.Status == TaskItemStatus.Done && !doneDates.ContainsKey(t.Id)))
            {
                doneDates[t.Id] = DateOnly.FromDateTime(t.UpdatedAt ?? t.CreatedAt);
            }
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var totalDays = end.DayNumber - start.DayNumber;
        var points = new List<BurndownPoint>();
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var dayIndex = d.DayNumber - start.DayNumber;
            var ideal = totalDays == 0 ? 0 : total * (1.0 - (double)dayIndex / totalDays);

            double? actual = null;
            if (d <= today)
            {
                var burned = doneDates.Where(kv => kv.Value <= d)
                    .Sum(kv => tasks.First(t => t.Id == kv.Key).Points);
                actual = total - burned;
            }
            points.Add(new BurndownPoint(d, Math.Round(ideal, 2), actual));
        }

        return Result<BurndownDto>.Ok(new BurndownDto(sprint.Id, sprint.Name, start, end, total, points));
    }
}
