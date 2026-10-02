using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Application.Features.Tasks.Mappings;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.MoveTask;

public interface IMoveTaskHandler
{
    Task<Result<TaskDto>> Handle(MoveTaskCommand command, CancellationToken ct);
}

/// <summary>Backs the Kanban drag-and-drop endpoint. Uses fractional-index positioning (see
/// planning.md §9 risks): the moved task's new Position is computed as the midpoint between its
/// new neighbors, so a single move only ever touches the ONE row being moved — never a
/// column-wide renumber — which is what keeps concurrent drags from colliding. If repeated
/// moves have squeezed two neighbors' positions too close together for a further float midpoint
/// to be distinct, the column is reindexed (rare, self-healing) before computing the move.</summary>
public class MoveTaskHandler : IMoveTaskHandler
{
    private const double MinGap = 1e-6;

    private readonly IApplicationDbContext _db;
    private readonly IActivityLogger _activity;
    private readonly INotificationDispatcher _notifications;

    public MoveTaskHandler(IApplicationDbContext db, IActivityLogger activity, INotificationDispatcher notifications)
    {
        _db = db;
        _activity = activity;
        _notifications = notifications;
    }

    public async Task<Result<TaskDto>> Handle(MoveTaskCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result<TaskDto>.Fail("Task not found.");

        var column = await _db.BoardColumns.FirstOrDefaultAsync(c => c.Id == command.TargetColumnId, ct);
        if (column is null || column.ProjectId != task.ProjectId)
        {
            return Result<TaskDto>.Fail("Board column not found in this project.");
        }

        if (column.WipLimit is not null)
        {
            var currentCount = await _db.Tasks.AsNoTracking()
                .CountAsync(t => t.BoardColumnId == command.TargetColumnId && t.Id != command.TaskId, ct);
            if (currentCount >= column.WipLimit && task.BoardColumnId != command.TargetColumnId)
            {
                return Result<TaskDto>.Fail($"Column \"{column.Name}\" is at its WIP limit ({column.WipLimit}).");
            }
        }

        var siblingPositions = await _db.Tasks.AsNoTracking()
            .Where(t => t.BoardColumnId == command.TargetColumnId && t.Id != command.TaskId)
            .OrderBy(t => t.Position)
            .Select(t => t.Position)
            .ToListAsync(ct);

        var index = Math.Clamp(command.TargetIndex, 0, siblingPositions.Count);
        var (lower, upper) = (
            index == 0 ? (double?)null : siblingPositions[index - 1],
            index == siblingPositions.Count ? (double?)null : siblingPositions[index]);

        if (lower is not null && upper is not null && upper.Value - lower.Value < MinGap)
        {
            await ReindexColumnAsync(command.TargetColumnId, ct);
            siblingPositions = await _db.Tasks.AsNoTracking()
                .Where(t => t.BoardColumnId == command.TargetColumnId && t.Id != command.TaskId)
                .OrderBy(t => t.Position).Select(t => t.Position).ToListAsync(ct);
            index = Math.Clamp(command.TargetIndex, 0, siblingPositions.Count);
            lower = index == 0 ? null : siblingPositions[index - 1];
            upper = index == siblingPositions.Count ? null : siblingPositions[index];
        }

        var newPosition = (lower, upper) switch
        {
            (null, null) => 1024.0,
            (null, not null) => upper!.Value - 1024.0,
            (not null, null) => lower!.Value + 1024.0,
            (not null, not null) => (lower!.Value + upper!.Value) / 2.0
        };

        var previousStatus = task.Status;
        task.BoardColumnId = command.TargetColumnId;
        task.Status = column.StatusMapping;
        task.Position = newPosition;

        await _db.SaveChangesAsync(ct);

        if (previousStatus != task.Status)
        {
            await _activity.LogAsync("Task", task.Id, "StatusChanged",
                new { Status = previousStatus.ToString() }, new { Status = task.Status.ToString() }, ct);

            var recipients = await RecipientsForTaskAsync(task.Id, task.AssigneeUserId, ct);
            await _notifications.DispatchAsync(
                recipients, NotificationType.TaskStatusChanged,
                $"{task.TaskKey} moved to {task.Status}", task.Title, "Task", task.Id, ct);
        }

        var dto = await _db.Tasks.AsNoTracking().Where(t => t.Id == task.Id).Select(TaskMappings.ToDto).FirstAsync(ct);
        return Result<TaskDto>.Ok(dto);
    }

    /// <summary>Everyone with a stake in a task: its watchers plus its current assignee.</summary>
    private async Task<List<Guid>> RecipientsForTaskAsync(Guid taskId, Guid? assigneeUserId, CancellationToken ct)
    {
        var recipients = await _db.TaskWatchers.AsNoTracking()
            .Where(w => w.TaskId == taskId).Select(w => w.UserId).ToListAsync(ct);
        if (assigneeUserId is not null) recipients.Add(assigneeUserId.Value);
        return recipients;
    }

    private async Task ReindexColumnAsync(Guid columnId, CancellationToken ct)
    {
        var tasks = await _db.Tasks.Where(t => t.BoardColumnId == columnId).OrderBy(t => t.Position).ToListAsync(ct);
        for (var i = 0; i < tasks.Count; i++)
        {
            tasks[i].Position = (i + 1) * 1024.0;
        }
        await _db.SaveChangesAsync(ct);
    }
}
