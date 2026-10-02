using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.TimeTracking.Dtos;
using ProjectTracker.Application.Features.TimeTracking.Mappings;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.TimeTracking.Commands.UpdateTimeEntry;

public record UpdateTimeEntryCommand(Guid TimeEntryId, DateOnly Date, int DurationMinutes, bool IsBillable, string? Description);

public interface IUpdateTimeEntryHandler
{
    Task<Result<TimeEntryDto>> Handle(UpdateTimeEntryCommand command, CancellationToken ct);
}

public class UpdateTimeEntryHandler : IUpdateTimeEntryHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateTimeEntryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<TimeEntryDto>> Handle(UpdateTimeEntryCommand command, CancellationToken ct)
    {
        var entry = await _db.TimeEntries.FirstOrDefaultAsync(e => e.Id == command.TimeEntryId, ct);
        if (entry is null) return Result<TimeEntryDto>.Fail("Time entry not found.");

        if (entry.UserId != _currentUser.UserId)
        {
            return Result<TimeEntryDto>.Fail("You can only edit your own time entries.");
        }
        if (entry.IsRunningTimer)
        {
            return Result<TimeEntryDto>.Fail("Stop the running timer before editing this entry.");
        }

        entry.Date = command.Date;
        entry.DurationMinutes = command.DurationMinutes;
        entry.IsBillable = command.IsBillable;
        entry.Description = command.Description?.Trim();

        var task = await _db.Tasks.FirstAsync(t => t.Id == entry.TaskId, ct);
        await _db.SaveChangesAsync(ct);

        await TaskHoursRecalculator.ApplyAsync(_db, task, ct);
        await _db.SaveChangesAsync(ct);

        var dto = await _db.TimeEntries.AsNoTracking().Where(e => e.Id == entry.Id)
            .Select(TimeEntryProjection.ToDto).FirstAsync(ct);
        return Result<TimeEntryDto>.Ok(dto);
    }
}
