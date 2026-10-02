using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Sprints.Commands.CancelSprint;
using ProjectTracker.Application.Features.Sprints.Commands.CompleteSprint;
using ProjectTracker.Application.Features.Sprints.Commands.CreateSprint;
using ProjectTracker.Application.Features.Sprints.Commands.DeleteSprint;
using ProjectTracker.Application.Features.Sprints.Commands.StartSprint;
using ProjectTracker.Application.Features.Sprints.Commands.UpdateSprint;
using ProjectTracker.Application.Features.Reports.Queries.GetBurndown;
using ProjectTracker.Application.Features.Sprints.Queries.GetSprint;
using ProjectTracker.Application.Features.Sprints.Queries.GetSprints;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sprints")]
[Authorize]
public class SprintsController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetSprints([FromQuery] Guid projectId, [FromQuery] SprintStatus? status, [FromServices] IGetSprintsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetSprintsQuery(projectId, status), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetSprint(Guid id, [FromServices] IGetSprintHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetSprintQuery(id), ct);
        return FromResult(result);
    }

    [HttpPost]
    [Authorize(Policy = PermissionKeys.SprintCreate)]
    public async Task<IActionResult> CreateSprint([FromBody] CreateSprintRequest request, [FromServices] ICreateSprintHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CreateSprintCommand(
            request.ProjectId, request.Name, request.Goal, request.StartDate, request.EndDate), ct);
        return FromResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionKeys.SprintCreate)]
    public async Task<IActionResult> UpdateSprint(Guid id, [FromBody] UpdateSprintRequest request, [FromServices] IUpdateSprintHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateSprintCommand(
            id, request.Name, request.Goal, request.StartDate, request.EndDate), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/start")]
    [Authorize(Policy = PermissionKeys.SprintStart)]
    public async Task<IActionResult> StartSprint(Guid id, [FromServices] IStartSprintHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new StartSprintCommand(id), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/complete")]
    [Authorize(Policy = PermissionKeys.SprintComplete)]
    public async Task<IActionResult> CompleteSprint(Guid id, [FromBody] CompleteSprintRequest? request, [FromServices] ICompleteSprintHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CompleteSprintCommand(id, request?.MoveIncompleteToBacklog ?? true), ct);
        return FromResult(result);
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = PermissionKeys.SprintComplete)]
    public async Task<IActionResult> CancelSprint(Guid id, [FromServices] ICancelSprintHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CancelSprintCommand(id), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionKeys.SprintCreate)]
    public async Task<IActionResult> DeleteSprint(Guid id, [FromServices] IDeleteSprintHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteSprintCommand(id), ct);
        return FromResult(result);
    }

    [HttpGet("{id:guid}/burndown")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetBurndown(Guid id, [FromServices] IGetBurndownHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetBurndownQuery(id), ct);
        return FromResult(result);
    }
}

public record CreateSprintRequest(Guid ProjectId, string Name, string? Goal, DateOnly? StartDate, DateOnly? EndDate);
public record UpdateSprintRequest(string Name, string? Goal, DateOnly? StartDate, DateOnly? EndDate);
public record CompleteSprintRequest(bool MoveIncompleteToBacklog);
