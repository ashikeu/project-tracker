using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.BulkUpdateTasks;

public interface IBulkUpdateTasksHandler
{
    Task<Result<int>> Handle(BulkUpdateTasksCommand command, CancellationToken ct);
}

public class BulkUpdateTasksHandler : IBulkUpdateTasksHandler
{
    private readonly IApplicationDbContext _db;

    public BulkUpdateTasksHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<int>> Handle(BulkUpdateTasksCommand command, CancellationToken ct)
    {
        var tasks = await _db.Tasks.Where(t => command.TaskIds.Contains(t.Id)).ToListAsync(ct);
        if (tasks.Count == 0) return Result<int>.Fail("No matching tasks found.");

        if (command.ChangeAssignee && command.AssigneeUserId is not null)
        {
            var orgId = tasks[0].OrganizationId;
            var isMember = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == orgId && ou.UserId == command.AssigneeUserId, ct);
            if (!isMember)
            {
                return Result<int>.FailValidation(new[] { new ValidationError(nameof(command.AssigneeUserId), "The specified assignee is not a member of this organization.") });
            }
        }

        if (command.ChangeSprint && command.SprintId is not null)
        {
            var sprint = await _db.Sprints.AsNoTracking().FirstOrDefaultAsync(s => s.Id == command.SprintId, ct);
            if (sprint is null)
            {
                return Result<int>.FailValidation(new[] { new ValidationError(nameof(command.SprintId), "The specified sprint was not found.") });
            }
            if (sprint.Status is SprintStatus.Completed or SprintStatus.Cancelled)
            {
                return Result<int>.FailValidation(new[] { new ValidationError(nameof(command.SprintId), "Tasks cannot be assigned to a completed or cancelled sprint.") });
            }
            if (tasks.Any(t => t.ProjectId != sprint.ProjectId))
            {
                return Result<int>.FailValidation(new[] { new ValidationError(nameof(command.SprintId), "All selected tasks must belong to the sprint's project.") });
            }
        }

        foreach (var task in tasks)
        {
            if (command.ChangeAssignee) task.AssigneeUserId = command.AssigneeUserId;
            if (command.Status is not null) task.Status = command.Status.Value;
            if (command.Priority is not null) task.Priority = command.Priority.Value;
            if (command.ChangeEpic) task.EpicId = command.EpicId;
            if (command.ChangeSprint) task.SprintId = command.SprintId;

            if (command.AddLabelId is not null)
            {
                var alreadyApplied = await _db.TaskLabels.AsNoTracking()
                    .AnyAsync(tl => tl.TaskId == task.Id && tl.LabelId == command.AddLabelId, ct);
                if (!alreadyApplied)
                {
                    _db.TaskLabels.Add(new TaskLabel { TaskId = task.Id, LabelId = command.AddLabelId.Value });
                }
            }
        }

        await _db.SaveChangesAsync(ct);
        return Result<int>.Ok(tasks.Count, $"{tasks.Count} task(s) updated.");
    }
}
