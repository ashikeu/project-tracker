using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Reports.Dtos;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Reports.Queries.GetTimeTrackingReport;

public record GetTimeTrackingReportQuery(Guid ProjectId, DateOnly? From, DateOnly? To);

public interface IGetTimeTrackingReportHandler
{
    Task<Result<TimeTrackingReportDto>> Handle(GetTimeTrackingReportQuery query, CancellationToken ct);
}

/// <summary>Logged vs billable hours per user across a project's tasks, optionally bounded by an
/// entry-date range. Running timers are excluded (their duration isn't finalized).</summary>
public class GetTimeTrackingReportHandler : IGetTimeTrackingReportHandler
{
    private readonly IApplicationDbContext _db;

    public GetTimeTrackingReportHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<TimeTrackingReportDto>> Handle(GetTimeTrackingReportQuery query, CancellationToken ct)
    {
        var projectExists = await _db.Projects.AsNoTracking().AnyAsync(p => p.Id == query.ProjectId, ct);
        if (!projectExists) return Result<TimeTrackingReportDto>.Fail("Project not found.");

        var entries = _db.TimeEntries.AsNoTracking()
            .Where(e => e.Task.ProjectId == query.ProjectId && !e.IsRunningTimer);
        if (query.From is not null) entries = entries.Where(e => e.Date >= query.From);
        if (query.To is not null) entries = entries.Where(e => e.Date <= query.To);

        var grouped = await entries
            .GroupBy(e => new { e.UserId, Name = e.User.FirstName + " " + e.User.LastName })
            .Select(g => new
            {
                g.Key.UserId,
                g.Key.Name,
                EntryCount = g.Count(),
                Minutes = g.Sum(e => e.DurationMinutes),
                BillableMinutes = g.Sum(e => e.IsBillable ? e.DurationMinutes : 0)
            })
            .ToListAsync(ct);

        var rows = grouped.Select(g => new TimeTrackingReportRow(
            g.UserId, g.Name, g.EntryCount, Math.Round(g.Minutes / 60m, 2), Math.Round(g.BillableMinutes / 60m, 2)))
            .OrderByDescending(r => r.LoggedHours)
            .ToList();

        return Result<TimeTrackingReportDto>.Ok(new TimeTrackingReportDto(
            query.ProjectId, query.From, query.To,
            rows.Sum(r => r.LoggedHours), rows.Sum(r => r.BillableHours), rows));
    }
}
