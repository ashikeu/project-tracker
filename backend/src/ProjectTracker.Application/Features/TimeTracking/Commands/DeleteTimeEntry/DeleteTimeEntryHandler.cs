using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.TimeTracking.Commands.DeleteTimeEntry;

public record DeleteTimeEntryCommand(Guid TimeEntryId);

public interface IDeleteTimeEntryHandler
{
    Task<Result> Handle(DeleteTimeEntryCommand command, CancellationToken ct);
}

public class DeleteTimeEntryHandler : IDeleteTimeEntryHandler
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteTimeEntryHandler(IApplicationDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteTimeEntryCommand command, CancellationToken ct)
    {
        var entry = await _db.TimeEntries.FirstOrDefaultAsync(e => e.Id == command.TimeEntryId, ct);
        if (entry is null) return Result.Fail("Time entry not found.");

        if (entry.UserId != _currentUser.UserId)
        {
            return Result.Fail("You can only delete your own time entries.");
        }

        var task = await _db.Tasks.FirstAsync(t => t.Id == entry.TaskId, ct);
        _db.TimeEntries.Remove(entry);
        await _db.SaveChangesAsync(ct);

        await TaskHoursRecalculator.ApplyAsync(_db, task, ct);
        await _db.SaveChangesAsync(ct);

        return Result.Ok("Time entry deleted.");
    }
}
