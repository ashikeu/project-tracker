using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.TimeTracking.Dtos;
using ProjectTracker.Application.Features.TimeTracking.Mappings;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.TimeTracking.Commands.CreateTimeEntry;

public record CreateTimeEntryCommand(Guid TaskId, DateOnly Date, int DurationMinutes, bool IsBillable, string? Description);

public interface ICreateTimeEntryHandler
{
    Task<Result<TimeEntryDto>> Handle(CreateTimeEntryCommand command, CancellationToken ct);
}

public class CreateTimeEntryHandler : ICreateTimeEntryHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateTimeEntryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<TimeEntryDto>> Handle(CreateTimeEntryCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result<TimeEntryDto>.Fail("Task not found.");

        var entry = new TimeEntry
        {
            OrganizationId = task.OrganizationId,
            TaskId = task.Id,
            UserId = _currentUser.UserId!.Value,
            Date = command.Date,
            DurationMinutes = command.DurationMinutes,
            IsBillable = command.IsBillable,
            Description = command.Description?.Trim(),
            IsRunningTimer = false
        };
        _db.TimeEntries.Add(entry);
        await _db.SaveChangesAsync(ct);

        await TaskHoursRecalculator.ApplyAsync(_db, task, ct);
        await _db.SaveChangesAsync(ct);

        var dto = await _db.TimeEntries.AsNoTracking().Where(e => e.Id == entry.Id)
            .Select(TimeEntryProjection.ToDto).FirstAsync(ct);
        return Result<TimeEntryDto>.Ok(dto);
    }
}
