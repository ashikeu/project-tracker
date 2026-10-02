using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.RemoveWatcher;

public record RemoveWatcherCommand(Guid TaskId, Guid UserId);

public interface IRemoveWatcherHandler
{
    Task<Result> Handle(RemoveWatcherCommand command, CancellationToken ct);
}

public class RemoveWatcherHandler : IRemoveWatcherHandler
{
    private readonly IApplicationDbContext _db;

    public RemoveWatcherHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(RemoveWatcherCommand command, CancellationToken ct)
    {
        var watcher = await _db.TaskWatchers.FirstOrDefaultAsync(w => w.TaskId == command.TaskId && w.UserId == command.UserId, ct);
        if (watcher is null) return Result.Ok("Not watching.");

        _db.TaskWatchers.Remove(watcher);
        await _db.SaveChangesAsync(ct);
        return Result.Ok("Stopped watching task.");
    }
}
