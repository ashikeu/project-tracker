using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Backlog.Commands.MoveBacklogItem;
using ProjectTracker.Application.Features.Backlog.Queries.GetBacklog;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/backlog")]
[Authorize]
public class BacklogController : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionKeys.TaskView)]
    public async Task<IActionResult> GetBacklog([FromQuery] Guid projectId, [FromQuery] int page, [FromQuery] int pageSize, [FromServices] IGetBacklogHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetBacklogQuery(projectId, page <= 0 ? 1 : page, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost("reorder")]
    [Authorize(Policy = PermissionKeys.TaskMove)]
    public async Task<IActionResult> Reorder([FromBody] MoveBacklogItemCommand command, [FromServices] IMoveBacklogItemHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(command, ct);
        return FromResult(result);
    }
}
