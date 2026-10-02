using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.AddWatcher;

public record AddWatcherCommand(Guid TaskId, Guid UserId);

public interface IAddWatcherHandler
{
    Task<Result> Handle(AddWatcherCommand command, CancellationToken ct);
}

public class AddWatcherHandler : IAddWatcherHandler
{
    private readonly IApplicationDbContext _db;

    public AddWatcherHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(AddWatcherCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result.Fail("Task not found.");

        var isMember = await _db.OrganizationUsers.AsNoTracking()
            .AnyAsync(ou => ou.OrganizationId == task.OrganizationId && ou.UserId == command.UserId, ct);
        if (!isMember) return Result.Fail("This user is not a member of the organization.");

        var alreadyWatching = await _db.TaskWatchers.AsNoTracking().AnyAsync(w => w.TaskId == command.TaskId && w.UserId == command.UserId, ct);
        if (alreadyWatching) return Result.Ok("Already watching.");

        _db.TaskWatchers.Add(new TaskWatcher { TaskId = command.TaskId, UserId = command.UserId });
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Watching task.");
    }
}
