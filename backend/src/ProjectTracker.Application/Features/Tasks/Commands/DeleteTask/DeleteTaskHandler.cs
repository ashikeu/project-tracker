using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid TaskId);

public interface IDeleteTaskHandler
{
    Task<Result> Handle(DeleteTaskCommand command, CancellationToken ct);
}

public class DeleteTaskHandler : IDeleteTaskHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IActivityLogger _activity;

    public DeleteTaskHandler(IApplicationDbContext db, IActivityLogger activity)
    {
        _db = db;
        _activity = activity;
    }

    public async Task<Result> Handle(DeleteTaskCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result.Fail("Task not found.");

        // Soft-deleting the parent cascades (in the business sense) to its subtasks too — a
        // subtask orphaned from a deleted parent would be confusing to surface anywhere.
        var subtasks = await _db.Tasks.Where(t => t.ParentTaskId == command.TaskId).ToListAsync(ct);
        foreach (var subtask in subtasks) subtask.IsDeleted = true;

        task.IsDeleted = true;
        await _db.SaveChangesAsync(ct);

        await _activity.LogAsync("Task", task.Id, "Deleted", new { task.TaskKey, task.Title }, null, ct);
        return Result.Ok("Task deleted.");
    }
}
