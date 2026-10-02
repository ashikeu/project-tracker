using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Reports.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Reports.Queries.GetTeamProductivityReport;

public record GetTeamProductivityReportQuery(Guid ProjectId, DateOnly? From, DateOnly? To);

public interface IGetTeamProductivityReportHandler
{
    Task<Result<TeamProductivityDto>> Handle(GetTeamProductivityReportQuery query, CancellationToken ct);
}

/// <summary>Per-person completed work in a window, crediting whoever moved each task to Done (from
/// ActivityLog), plus hours they logged in the window. Defaults to the last 30 days.</summary>
public class GetTeamProductivityReportHandler : IGetTeamProductivityReportHandler
{
    private readonly IApplicationDbContext _db;

    public GetTeamProductivityReportHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TeamProductivityDto>> Handle(GetTeamProductivityReportQuery query, CancellationToken ct)
    {
        var projectExists = await _db.Projects.AsNoTracking().AnyAsync(p => p.Id == query.ProjectId, ct);
        if (!projectExists) return Result<TeamProductivityDto>.Fail("Project not found.");

        var to = query.To ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var from = query.From ?? to.AddDays(-30);
        var fromDt = from.ToDateTime(TimeOnly.MinValue);
        var toDt = to.ToDateTime(TimeOnly.MaxValue);

        // Task ids in this project (for scoping the activity-log join) with their points.
        var projectTaskPoints = await _db.Tasks.AsNoTracking()
            .Where(t => t.ProjectId == query.ProjectId)
            .Select(t => new { t.Id, Points = t.StoryPoints ?? 0 })
            .ToDictionaryAsync(t => t.Id, t => t.Points, ct);

        // "Reached Done" activity within the window, attributed to the acting user.
        var doneLogs = await _db.ActivityLogs.AsNoTracking()
            .Where(a => a.EntityType == "Task" && a.UserId != null
                        && a.NewValueJson != null && a.NewValueJson.Contains("\"Status\":\"Done\"")
                        && a.CreatedAt >= fromDt && a.CreatedAt <= toDt)
            .Select(a => new { a.EntityId, UserId = a.UserId!.Value, a.CreatedAt })
            .ToListAsync(ct);

        // One credit per (user, task) — a task toggled Done twice by the same person counts once.
        var completions = doneLogs
            .Where(l => projectTaskPoints.ContainsKey(l.EntityId))
            .GroupBy(l => new { l.UserId, l.EntityId })
            .Select(g => new { g.Key.UserId, g.Key.EntityId })
            .ToList();

        var loggedByUser = await _db.TimeEntries.AsNoTracking()
            .Where(e => e.Task.ProjectId == query.ProjectId && !e.IsRunningTimer
                        && e.Date >= from && e.Date <= to)
            .GroupBy(e => e.UserId)
            .Select(g => new { UserId = g.Key, Minutes = g.Sum(e => e.DurationMinutes) })
            .ToDictionaryAsync(x => x.UserId, x => x.Minutes, ct);

        var userIds = completions.Select(c => c.UserId).Concat(loggedByUser.Keys).Distinct().ToList();
        var names = await _db.Users.AsNoTracking()
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, Name = u.FirstName + " " + u.LastName })
            .ToDictionaryAsync(u => u.Id, u => u.Name, ct);

        var rows = userIds.Select(uid =>
        {
            var userCompletions = completions.Where(c => c.UserId == uid).ToList();
            var points = userCompletions.Sum(c => projectTaskPoints[c.EntityId]);
            var hours = loggedByUser.TryGetValue(uid, out var m) ? Math.Round(m / 60m, 2) : 0m;
            return new TeamProductivityRow(uid, names.GetValueOrDefault(uid, "Unknown"), userCompletions.Count, points, hours);
        })
        .OrderByDescending(r => r.CompletedTasks)
        .ToList();

        return Result<TeamProductivityDto>.Ok(new TeamProductivityDto(query.ProjectId, from, to, rows));
    }
}
