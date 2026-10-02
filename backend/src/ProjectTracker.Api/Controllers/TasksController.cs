using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Application.Features.Tasks.Commands.AddChecklistItem;
using ProjectTracker.Application.Features.Tasks.Commands.AddDependency;
using ProjectTracker.Application.Features.Tasks.Commands.AddTaskLabel;
using ProjectTracker.Application.Features.Tasks.Commands.AddWatcher;
using ProjectTracker.Application.Features.Tasks.Commands.AssignTask;
using ProjectTracker.Application.Features.Tasks.Commands.BulkUpdateTasks;
using ProjectTracker.Application.Features.Tasks.Commands.CreateTask;
using ProjectTracker.Application.Features.Tasks.Commands.DeleteChecklistItem;
using ProjectTracker.Application.Features.Tasks.Commands.DeleteTask;
using ProjectTracker.Application.Features.Tasks.Commands.MoveTask;
using ProjectTracker.Application.Features.Tasks.Commands.RemoveDependency;
using ProjectTracker.Application.Features.Tasks.Commands.RemoveTaskLabel;
using ProjectTracker.Application.Features.Tasks.Commands.RemoveWatcher;
using ProjectTracker.Application.Features.Tasks.Commands.UpdateChecklistItem;
using ProjectTracker.Application.Features.Tasks.Commands.UpdateTask;
using ProjectTracker.Application.Features.Activity.Queries.GetActivity;
using ProjectTracker.Application.Features.Tasks.Queries.GetTask;
using ProjectTracker.Application.Features.Tasks.Queries.GetTasks;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/tasks")]
[Authorize]
public class TasksController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUser;

    public TasksController(ICurrentUserService currentUser) => _currentUser = currentUser;

    [HttpGet]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> GetTasks([FromQuery] TaskListQuery q, [FromServices] IGetTasksHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetTasksQuery(
            q.ProjectId, q.EpicId, q.SprintId, q.Status, q.Priority, q.TaskType, q.AssigneeUserId, q.ReporterUserId,
            q.TeamId, q.LabelId, q.DueBefore, q.DueAfter, q.Search, q.UnassignedOnly, q.TopLevelOnly,
            q.SortBy, q.SortDescending, q.Page, q.PageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> GetTask(Guid id, [FromServices] IGetTaskHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetTaskQuery(id), ct);
        return FromResult(result);
    }

    [HttpGet("{id:guid}/activity")]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> GetActivity(Guid id, [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetActivityHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetActivityQuery("Task", id, page <= 0 ? 1 : page, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    [Authorize(Policy = PermissionKeys.TaskCreate)]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request, [FromServices] ICreateTaskHandler handler, CancellationToken ct)
    {
        if (_currentUser.UserId is null) return Unauthorized();
        var result = await handler.Handle(new CreateTaskCommand(
            request.ProjectId, _currentUser.UserId.Value, request.EpicId, request.ParentTaskId,
            request.Title, request.Description, request.TaskType, request.Priority,
            request.AssigneeUserId, request.TeamId, request.StoryPoints, request.EstimatedHours,
            request.StartDate, request.DueDate, request.LabelIds), ct);
        return FromResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskRequest request, [FromServices] IUpdateTaskHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateTaskCommand(
            id, request.Title, request.Description, request.EpicId, request.TaskType, request.Status,
            request.Priority, request.TeamId, request.StoryPoints, request.EstimatedHours,
            request.RemainingHours, request.StartDate, request.DueDate), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionKeys.TaskDelete)]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IDeleteTaskHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteTaskCommand(id), ct);
        return FromResult(result);
    }

    /// <summary>Kanban drag-and-drop: move a task into a column at a given position. Updates
    /// Status (via the column's StatusMapping), BoardColumnId and Position together.</summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = PermissionKeys.TaskMove)]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveTaskRequest request, [FromServices] IMoveTaskHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new MoveTaskCommand(id, request.ColumnId, request.Index), ct);
        return FromResult(result);
    }

    [HttpPatch("{id:guid}/assign")]
    [Authorize(Policy = PermissionKeys.TaskAssign)]
    public async Task<IActionResult> Assign(Guid id, [FromBody] AssignTaskRequest request, [FromServices] IAssignTaskHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new AssignTaskCommand(id, request.AssigneeUserId), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/watchers")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> AddWatcher(Guid id, [FromBody] WatcherRequest request, [FromServices] IAddWatcherHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new AddWatcherCommand(id, request.UserId), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}/watchers/{userId:guid}")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> RemoveWatcher(Guid id, Guid userId, [FromServices] IRemoveWatcherHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new RemoveWatcherCommand(id, userId), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/checklist")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> AddChecklistItem(Guid id, [FromBody] AddChecklistItemRequest request, [FromServices] IAddChecklistItemHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new AddChecklistItemCommand(id, request.Text), ct);
        return FromResult(result);
    }

    [HttpPut("{id:guid}/checklist/{itemId:guid}")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> UpdateChecklistItem(Guid id, Guid itemId, [FromBody] UpdateChecklistItemRequest request, [FromServices] IUpdateChecklistItemHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateChecklistItemCommand(itemId, request.Text, request.IsDone, request.Order), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}/checklist/{itemId:guid}")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> DeleteChecklistItem(Guid id, Guid itemId, [FromServices] IDeleteChecklistItemHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteChecklistItemCommand(itemId), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/labels/{labelId:guid}")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> AddLabel(Guid id, Guid labelId, [FromServices] IAddTaskLabelHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new AddTaskLabelCommand(id, labelId), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}/labels/{labelId:guid}")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> RemoveLabel(Guid id, Guid labelId, [FromServices] IRemoveTaskLabelHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new RemoveTaskLabelCommand(id, labelId), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/dependencies")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> AddDependency(Guid id, [FromBody] AddDependencyRequest request, [FromServices] IAddDependencyHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new AddDependencyCommand(id, request.DependsOnTaskId, request.Type), ct);
        return FromResult(result);
    }

    [HttpDelete("dependencies/{dependencyId:guid}")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> RemoveDependency(Guid dependencyId, [FromServices] IRemoveDependencyHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new RemoveDependencyCommand(dependencyId), ct);
        return FromResult(result);
    }

    [HttpPost("bulk")]
    [Authorize(Policy = PermissionKeys.TaskUpdate)]
    public async Task<IActionResult> BulkUpdate([FromBody] BulkUpdateTasksCommand command, [FromServices] IBulkUpdateTasksHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(command, ct);
        return FromResult(result);
    }
}

public class TaskListQuery
{
    public Guid? ProjectId { get; set; }
    public Guid? EpicId { get; set; }
    public Guid? SprintId { get; set; }
    public TaskItemStatus? Status { get; set; }
    public Priority? Priority { get; set; }
    public TaskType? TaskType { get; set; }
    public Guid? AssigneeUserId { get; set; }
    public Guid? ReporterUserId { get; set; }
    public Guid? TeamId { get; set; }
    public Guid? LabelId { get; set; }
    public DateOnly? DueBefore { get; set; }
    public DateOnly? DueAfter { get; set; }
    public string? Search { get; set; }
    public bool UnassignedOnly { get; set; }
    public bool TopLevelOnly { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public record CreateTaskRequest(
    Guid ProjectId, Guid? EpicId, Guid? ParentTaskId, string Title, string? Description,
    TaskType TaskType, Priority Priority, Guid? AssigneeUserId, Guid? TeamId, int? StoryPoints,
    decimal? EstimatedHours, DateOnly? StartDate, DateOnly? DueDate, IReadOnlyList<Guid>? LabelIds);

public record UpdateTaskRequest(
    string Title, string? Description, Guid? EpicId, TaskType TaskType, TaskItemStatus Status,
    Priority Priority, Guid? TeamId, int? StoryPoints, decimal? EstimatedHours,
    decimal? RemainingHours, DateOnly? StartDate, DateOnly? DueDate);

public record MoveTaskRequest(Guid ColumnId, int Index);
public record AssignTaskRequest(Guid? AssigneeUserId);
public record WatcherRequest(Guid UserId);
public record AddChecklistItemRequest(string Text);
public record UpdateChecklistItemRequest(string Text, bool IsDone, int Order);
public record AddDependencyRequest(Guid DependsOnTaskId, TaskDependencyType Type);
