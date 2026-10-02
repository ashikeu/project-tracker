using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Dtos;
using ProjectTracker.Application.Features.Tasks.Mappings;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Application.Features.Tasks.Commands.CreateTask;

public interface ICreateTaskHandler
{
    Task<Result<TaskDto>> Handle(CreateTaskCommand command, CancellationToken ct);
}

public class CreateTaskHandler : ICreateTaskHandler
{
    private readonly IApplicationDbContext _db;
    private readonly IActivityLogger _activity;

    public CreateTaskHandler(IApplicationDbContext db, IActivityLogger activity)
    {
        _db = db;
        _activity = activity;
    }

    public async Task<Result<TaskDto>> Handle(CreateTaskCommand command, CancellationToken ct)
    {
        var project = await _db.Projects.FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);
        if (project is null) return Result<TaskDto>.Fail("Project not found.");

        if (command.EpicId is not null)
        {
            var epicValid = await _db.Epics.AsNoTracking().AnyAsync(e => e.Id == command.EpicId && e.ProjectId == command.ProjectId, ct);
            if (!epicValid)
            {
                return Result<TaskDto>.FailValidation(new[] { new ValidationError(nameof(command.EpicId), "Epic not found in this project.") });
            }
        }

        TaskItem? parent = null;
        if (command.ParentTaskId is not null)
        {
            parent = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == command.ParentTaskId && t.ProjectId == command.ProjectId, ct);
            if (parent is null)
            {
                return Result<TaskDto>.FailValidation(new[] { new ValidationError(nameof(command.ParentTaskId), "Parent task not found in this project.") });
            }
            if (parent.ParentTaskId is not null)
            {
                return Result<TaskDto>.FailValidation(new[] { new ValidationError(nameof(command.ParentTaskId), "A subtask cannot itself have subtasks.") });
            }
        }

        if (command.AssigneeUserId is not null)
        {
            var assigneeIsMember = await _db.OrganizationUsers.AsNoTracking()
                .AnyAsync(ou => ou.OrganizationId == project.OrganizationId && ou.UserId == command.AssigneeUserId, ct);
            if (!assigneeIsMember)
            {
                return Result<TaskDto>.FailValidation(new[] { new ValidationError(nameof(command.AssigneeUserId), "The specified assignee is not a member of this organization.") });
            }
        }

        if (command.LabelIds is { Count: > 0 })
        {
            var validLabelCount = await _db.Labels.AsNoTracking().CountAsync(l => command.LabelIds.Contains(l.Id) && l.ProjectId == command.ProjectId, ct);
            if (validLabelCount != command.LabelIds.Count)
            {
                return Result<TaskDto>.FailValidation(new[] { new ValidationError(nameof(command.LabelIds), "One or more labels were not found in this project.") });
            }
        }

        // Transactional task-key assignment: NextTaskNumber is protected by Project.RowVersion,
        // so a concurrent create against the same project surfaces as a clear, retryable
        // conflict instead of silently issuing a duplicate/skipped task number.
        var assignedNumber = project.NextTaskNumber;
        project.NextTaskNumber++;

        // Fractional-index position: append to the end of the project's backlog ordering.
        var maxPosition = await _db.Tasks.AsNoTracking()
            .Where(t => t.ProjectId == command.ProjectId && t.ParentTaskId == null)
            .Select(t => (double?)t.Position).MaxAsync(ct) ?? 0;

        var task = new TaskItem
        {
            OrganizationId = project.OrganizationId,
            ProjectId = command.ProjectId,
            TaskKey = $"{project.Key}-{assignedNumber}",
            EpicId = command.EpicId,
            ParentTaskId = command.ParentTaskId,
            Title = command.Title.Trim(),
            Description = command.Description?.Trim(),
            TaskType = command.TaskType,
            Priority = command.Priority,
            AssigneeUserId = command.AssigneeUserId,
            ReporterUserId = command.ReporterUserId,
            TeamId = command.TeamId,
            StoryPoints = command.StoryPoints,
            EstimatedHours = command.EstimatedHours,
            RemainingHours = command.EstimatedHours,
            StartDate = command.StartDate,
            DueDate = command.DueDate,
            Position = command.ParentTaskId is null ? maxPosition + 1024 : 0
        };
        _db.Tasks.Add(task);

        if (command.LabelIds is { Count: > 0 })
        {
            foreach (var labelId in command.LabelIds.Distinct())
            {
                _db.TaskLabels.Add(new TaskLabel { Task = task, LabelId = labelId });
            }
        }

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return Result<TaskDto>.Fail("Another task was created in this project at the same moment. Please retry.");
        }

        await _activity.LogAsync("Task", task.Id, "Created", null, new { task.TaskKey, task.Title }, ct);

        var dto = await _db.Tasks.AsNoTracking().Where(t => t.Id == task.Id).Select(TaskMappings.ToDto).FirstAsync(ct);
        return Result<TaskDto>.Ok(dto);
    }
}
