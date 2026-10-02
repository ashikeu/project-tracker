using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.TimeTracking.Dtos;
using ProjectTracker.Application.Features.TimeTracking.Mappings;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.TimeTracking.Commands.StartTimer;

public record StartTimerCommand(Guid TaskId, bool IsBillable, string? Description);

public interface IStartTimerHandler
{
    Task<Result<TimeEntryDto>> Handle(StartTimerCommand command, CancellationToken ct);
}

public class StartTimerHandler : IStartTimerHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public StartTimerHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<TimeEntryDto>> Handle(StartTimerCommand command, CancellationToken ct)
    {
        var userId = _currentUser.UserId!.Value;

        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result<TimeEntryDto>.Fail("Task not found.");

        // One running timer per user at a time (across all tasks) — a second concurrent timer would
        // make "how long am I tracking" ambiguous.
        var alreadyRunning = await _db.TimeEntries.AsNoTracking()
            .AnyAsync(e => e.UserId == userId && e.IsRunningTimer, ct);
        if (alreadyRunning)
        {
            return Result<TimeEntryDto>.Fail("You already have a running timer. Stop it before starting a new one.");
        }

        var entry = new TimeEntry
        {
            OrganizationId = task.OrganizationId,
            TaskId = task.Id,
            UserId = userId,
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            DurationMinutes = 0,
            IsBillable = command.IsBillable,
            Description = command.Description?.Trim(),
            IsRunningTimer = true,
            StartedAt = DateTime.UtcNow
        };
        _db.TimeEntries.Add(entry);
        await _db.SaveChangesAsync(ct);

        var dto = await _db.TimeEntries.AsNoTracking().Where(e => e.Id == entry.Id)
            .Select(TimeEntryProjection.ToDto).FirstAsync(ct);
        return Result<TimeEntryDto>.Ok(dto, "Timer started.");
    }
}
