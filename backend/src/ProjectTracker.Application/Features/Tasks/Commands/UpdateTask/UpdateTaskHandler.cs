using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Application.Features.Tasks.Mappings;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.UpdateTask;

public interface IUpdateTaskHandler
{
    Task<Result<TaskDto>> Handle(UpdateTaskCommand command, CancellationToken ct);
}

public class UpdateTaskHandler : IUpdateTaskHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IActivityLogger _activity;
    private readonly INotificationDispatcher _notifications;

    public UpdateTaskHandler(IApplicationDbContext db, IActivityLogger activity, INotificationDispatcher notifications)
    {
        _db = db;
        _activity = activity;
        _notifications = notifications;
    }

    public async Task<Result<TaskDto>> Handle(UpdateTaskCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result<TaskDto>.Fail("Task not found.");

        if (command.EpicId is not null)
        {
            var epicValid = await _db.Epics.AsNoTracking().AnyAsync(e => e.Id == command.EpicId && e.ProjectId == task.ProjectId, ct);
            if (!epicValid)
            {
                return Result<TaskDto>.FailValidation(new[] { new ValidationError(nameof(command.EpicId), "Epic not found in this project.") });
            }
        }

        var previousStatus = task.Status;

        task.Title = command.Title.Trim();
        task.Description = command.Description?.Trim();
        task.EpicId = command.EpicId;
        task.TaskType = command.TaskType;
        task.Status = command.Status;
        task.Priority = command.Priority;
        task.TeamId = command.TeamId;
        task.StoryPoints = command.StoryPoints;
        task.EstimatedHours = command.EstimatedHours;
        task.RemainingHours = command.RemainingHours;
        task.StartDate = command.StartDate;
        task.DueDate = command.DueDate;

        await _db.SaveChangesAsync(ct);

        await _activity.LogAsync("Task", task.Id, "Updated", null, new { task.Title, Status = task.Status.ToString() }, ct);

        // A status change made through the edit form (rather than a board drag) still notifies stakeholders.
        if (previousStatus != task.Status)
        {
            var recipients = await _db.TaskWatchers.AsNoTracking()
                .Where(w => w.TaskId == task.Id).Select(w => w.UserId).ToListAsync(ct);
            if (task.AssigneeUserId is not null) recipients.Add(task.AssigneeUserId.Value);
            await _notifications.DispatchAsync(recipients, NotificationType.TaskStatusChanged,
                $"{task.TaskKey} moved to {task.Status}", task.Title, "Task", task.Id, ct);
        }

        var dto = await _db.Tasks.AsNoTracking().Where(t => t.Id == task.Id).Select(TaskMappings.ToDto).FirstAsync(ct);
        return Result<TaskDto>.Ok(dto);
    }
}
