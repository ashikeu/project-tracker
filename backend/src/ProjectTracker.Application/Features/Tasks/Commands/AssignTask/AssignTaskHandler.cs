using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Application.Features.Tasks.Mappings;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.AssignTask;

public record AssignTaskCommand(Guid TaskId, Guid? AssigneeUserId);

public interface IAssignTaskHandler
{
    Task<Result<TaskDto>> Handle(AssignTaskCommand command, CancellationToken ct);
}

public class AssignTaskHandler : IAssignTaskHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IActivityLogger _activity;
    private readonly INotificationDispatcher _notifications;

    public AssignTaskHandler(IApplicationDbContext db, IActivityLogger activity, INotificationDispatcher notifications)
    {
        _db = db;
        _activity = activity;
        _notifications = notifications;
    }

    public async Task<Result<TaskDto>> Handle(AssignTaskCommand command, CancellationToken ct)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == command.TaskId, ct);
        if (task is null) return Result<TaskDto>.Fail("Task not found.");

        if (command.AssigneeUserId is not null)
        {
            var isMember = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == task.OrganizationId && ou.UserId == command.AssigneeUserId, ct);
            if (!isMember)
            {
                return Result<TaskDto>.FailValidation(new[] { new ValidationError(nameof(command.AssigneeUserId), "The specified assignee is not a member of this organization.") });
            }
        }

        var previousAssignee = task.AssigneeUserId;
        if (previousAssignee == command.AssigneeUserId)
        {
            // No-op assignment — return current state without logging/notifying.
            var unchanged = await _db.Tasks.AsNoTracking().Where(t => t.Id == task.Id).Select(TaskMappings.ToDto).FirstAsync(ct);
            return Result<TaskDto>.Ok(unchanged);
        }

        task.AssigneeUserId = command.AssigneeUserId;
        await _db.SaveChangesAsync(ct);

        await _activity.LogAsync("Task", task.Id, "Assigned",
            new { AssigneeUserId = previousAssignee }, new { task.AssigneeUserId }, ct);

        if (command.AssigneeUserId is not null)
        {
            await _notifications.DispatchAsync(
                new[] { command.AssigneeUserId.Value }, NotificationType.TaskAssigned,
                $"You were assigned {task.TaskKey}", task.Title, "Task", task.Id, ct);
        }

        var dto = await _db.Tasks.AsNoTracking().Where(t => t.Id == task.Id).Select(TaskMappings.ToDto).FirstAsync(ct);
        return Result<TaskDto>.Ok(dto);
    }
}
