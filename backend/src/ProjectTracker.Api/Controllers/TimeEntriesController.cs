using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.TimeTracking.Commands.CreateTimeEntry;
using ProjectTracker.Application.Features.TimeTracking.Commands.DeleteTimeEntry;
using ProjectTracker.Application.Features.TimeTracking.Commands.StartTimer;
using ProjectTracker.Application.Features.TimeTracking.Commands.StopTimer;
using ProjectTracker.Application.Features.TimeTracking.Commands.UpdateTimeEntry;
using ProjectTracker.Application.Features.TimeTracking.Queries.GetTaskTimeSummary;
using ProjectTracker.Application.Features.TimeTracking.Queries.GetTimeEntries;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Authorize]
public class TimeEntriesController : ApiControllerBase
{
    [HttpGet("api/v{version:apiVersion}/tasks/{taskId:guid}/time-entries")]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> GetTimeEntries(Guid taskId, [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetTimeEntriesHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetTimeEntriesQuery(taskId, page <= 0 ? 1 : page, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("api/v{version:apiVersion}/tasks/{taskId:guid}/time-entries/summary")]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> GetSummary(Guid taskId, [FromServices] IGetTaskTimeSummaryHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetTaskTimeSummaryQuery(taskId), ct);
        return FromResult(result);
    }

    [HttpPost("api/v{version:apiVersion}/tasks/{taskId:guid}/time-entries")]
    [Authorize(Policy = PermissionKeys.TimeLog)]
    public async Task<IActionResult> CreateTimeEntry(Guid taskId, [FromBody] CreateTimeEntryRequest request, [FromServices] ICreateTimeEntryHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CreateTimeEntryCommand(
            taskId, request.Date, request.DurationMinutes, request.IsBillable, request.Description), ct);
        return FromResult(result);
    }

    [HttpPost("api/v{version:apiVersion}/tasks/{taskId:guid}/time-entries/start")]
    [Authorize(Policy = PermissionKeys.TimeLog)]
    public async Task<IActionResult> StartTimer(Guid taskId, [FromBody] StartTimerRequest? request, [FromServices] IStartTimerHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new StartTimerCommand(taskId, request?.IsBillable ?? false, request?.Description), ct);
        return FromResult(result);
    }

    [HttpPost("api/v{version:apiVersion}/time-entries/stop")]
    [Authorize(Policy = PermissionKeys.TimeLog)]
    public async Task<IActionResult> StopTimer([FromServices] IStopTimerHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new StopTimerCommand(), ct);
        return FromResult(result);
    }

    [HttpPut("api/v{version:apiVersion}/time-entries/{id:guid}")]
    [Authorize(Policy = PermissionKeys.TimeEdit)]
    public async Task<IActionResult> UpdateTimeEntry(Guid id, [FromBody] UpdateTimeEntryRequest request, [FromServices] IUpdateTimeEntryHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateTimeEntryCommand(
            id, request.Date, request.DurationMinutes, request.IsBillable, request.Description), ct);
        return FromResult(result);
    }

    [HttpDelete("api/v{version:apiVersion}/time-entries/{id:guid}")]
    [Authorize(Policy = PermissionKeys.TimeEdit)]
    public async Task<IActionResult> DeleteTimeEntry(Guid id, [FromServices] IDeleteTimeEntryHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteTimeEntryCommand(id), ct);
        return FromResult(result);
    }
}

public record CreateTimeEntryRequest(DateOnly Date, int DurationMinutes, bool IsBillable, string? Description);
public record StartTimerRequest(bool IsBillable, string? Description);
public record UpdateTimeEntryRequest(DateOnly Date, int DurationMinutes, bool IsBillable, string? Description);
