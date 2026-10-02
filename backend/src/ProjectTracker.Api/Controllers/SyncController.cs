using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Sync.Commands.PushChanges;
using ProjectTracker.Application.Features.Sync.Dtos;
using ProjectTracker.Application.Features.Sync.Queries.PullChanges;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sync")]
[Authorize]
public class SyncController : ApiControllerBase
{
    [HttpPost("push")]
    public async Task<IActionResult> Push([FromBody] SyncPushRequest request, [FromServices] IPushChangesHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(request, ct);
        return FromResult(result);
    }

    [HttpGet("pull")]
    public async Task<IActionResult> Pull([FromQuery] long since, [FromQuery] int pageSize, [FromServices] IPullChangesHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new PullChangesQuery(since, pageSize), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }
}
