using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.TimeTracking.Dtos;
using ProjectTracker.Application.Features.TimeTracking.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.TimeTracking.Commands.StopTimer;

/// <summary>Stops the caller's currently running timer (there is at most one — see StartTimer).</summary>
public record StopTimerCommand;

public interface IStopTimerHandler
{
    Task<Result<TimeEntryDto>> Handle(StopTimerCommand command, CancellationToken ct);
}

public class StopTimerHandler : IStopTimerHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public StopTimerHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<TimeEntryDto>> Handle(StopTimerCommand command, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        var entry = await _db.TimeEntries.FirstOrDefaultAsync(e => e.UserId == userId && e.IsRunningTimer, ct);
        if (entry is null) return Result<TimeEntryDto>.Fail("You have no running timer.");

        var elapsed = DateTime.UtcNow - (entry.StartedAt ?? DateTime.UtcNow);
        // At least one minute so a quick start/stop still records something.
        entry.DurationMinutes = Math.Max(1, (int)Math.Round(elapsed.TotalMinutes));
        entry.IsRunningTimer = false;

        var task = await _db.Tasks.FirstAsync(t => t.Id == entry.TaskId, ct);
        await _db.SaveChangesAsync(ct);

        await TaskHoursRecalculator.ApplyAsync(_db, task, ct);
        await _db.SaveChangesAsync(ct);

        var dto = await _db.TimeEntries.AsNoTracking().Where(e => e.Id == entry.Id)
            .Select(TimeEntryProjection.ToDto).FirstAsync(ct);
        return Result<TimeEntryDto>.Ok(dto, "Timer stopped.");
    }
}
