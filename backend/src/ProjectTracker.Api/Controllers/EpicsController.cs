using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Epics.Commands.CreateEpic;
using ProjectTracker.Application.Features.Epics.Commands.DeleteEpic;
using ProjectTracker.Application.Features.Epics.Commands.UpdateEpic;
using ProjectTracker.Application.Features.Epics.Queries.GetEpic;
using ProjectTracker.Application.Features.Epics.Queries.GetEpics;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/epics")]
[Authorize]
public class EpicsController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetEpics([FromQuery] Guid projectId, [FromServices] IGetEpicsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetEpicsQuery(projectId), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetEpic(Guid id, [FromServices] IGetEpicHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetEpicQuery(id), ct);
        return FromResult(result);
    }

    [HttpPost]
    [Authorize(Policy = PermissionKeys.EpicCreate)]
    public async Task<IActionResult> CreateEpic([FromBody] CreateEpicRequest request, [FromServices] ICreateEpicHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CreateEpicCommand(
            request.ProjectId, request.Name, request.Description, request.OwnerUserId, request.Priority,
            request.Color, request.Icon, request.StartDate, request.EndDate), ct);
        return FromResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionKeys.EpicUpdate)]
    public async Task<IActionResult> UpdateEpic(Guid id, [FromBody] UpdateEpicRequest request, [FromServices] IUpdateEpicHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateEpicCommand(
            id, request.Name, request.Description, request.OwnerUserId, request.Status, request.Priority,
            request.Color, request.Icon, request.StartDate, request.EndDate), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = PermissionKeys.EpicDelete)]
    public async Task<IActionResult> Delete(Guid id, [FromServices] IDeleteEpicHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteEpicCommand(id), ct);
        return FromResult(result);
    }
}

public record CreateEpicRequest(Guid ProjectId, string Name, string? Description, Guid? OwnerUserId, Priority Priority, string? Color, string? Icon, DateOnly? StartDate, DateOnly? EndDate);
public record UpdateEpicRequest(string Name, string? Description, Guid? OwnerUserId, EpicStatus Status, Priority Priority, string? Color, string? Icon, DateOnly? StartDate, DateOnly? EndDate);
