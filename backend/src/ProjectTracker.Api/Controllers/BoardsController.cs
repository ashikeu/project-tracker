using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Boards.Commands.CreateBoardColumn;
using ProjectTracker.Application.Features.Boards.Commands.DeleteBoardColumn;
using ProjectTracker.Application.Features.Boards.Commands.ReorderBoardColumns;
using ProjectTracker.Application.Features.Boards.Commands.UpdateBoardColumn;
using ProjectTracker.Application.Features.Boards.Queries.GetBoardColumns;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/boards")]
[Authorize]
public class BoardsController : ApiControllerBase
{
    [HttpGet("{projectId:guid}/columns")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetColumns(Guid projectId, [FromServices] IGetBoardColumnsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetBoardColumnsQuery(projectId), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("{projectId:guid}/columns")]
    [Authorize(Policy = PermissionKeys.ProjectUpdate)]
    public async Task<IActionResult> CreateColumn(Guid projectId, [FromBody] CreateColumnRequest request, [FromServices] ICreateBoardColumnHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CreateBoardColumnCommand(projectId, request.Name, request.Color, request.WipLimit, request.StatusMapping, request.IsCompletionColumn), ct);
        return FromResult(result);
    }

    [HttpPut("columns/{columnId:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectUpdate)]
    public async Task<IActionResult> UpdateColumn(Guid columnId, [FromBody] UpdateColumnRequest request, [FromServices] IUpdateBoardColumnHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new UpdateBoardColumnCommand(columnId, request.Name, request.Color, request.WipLimit, request.StatusMapping, request.IsCompletionColumn), ct);
        return FromResult(result);
    }

    [HttpDelete("columns/{columnId:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectUpdate)]
    public async Task<IActionResult> DeleteColumn(Guid columnId, [FromServices] IDeleteBoardColumnHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteBoardColumnCommand(columnId), ct);
        return FromResult(result);
    }

    [HttpPut("{projectId:guid}/columns/reorder")]
    [Authorize(Policy = PermissionKeys.ProjectUpdate)]
    public async Task<IActionResult> Reorder(Guid projectId, [FromBody] ReorderColumnsRequest request, [FromServices] IReorderBoardColumnsHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new ReorderBoardColumnsCommand(projectId, request.OrderedColumnIds), ct);
        return FromResult(result);
    }
}

public record CreateColumnRequest(string Name, string Color, int? WipLimit, TaskItemStatus StatusMapping, bool IsCompletionColumn);
public record UpdateColumnRequest(string Name, string Color, int? WipLimit, TaskItemStatus StatusMapping, bool IsCompletionColumn);
public record ReorderColumnsRequest(IReadOnlyList<Guid> OrderedColumnIds);
