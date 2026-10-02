using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Dashboard.Queries.GetDashboard;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/dashboard")]
[Authorize]
public class DashboardController : ApiControllerBase
{
    [HttpGet("{projectId:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> GetDashboard(Guid projectId, [FromServices] IGetDashboardHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetDashboardQuery(projectId), ct);
        return FromResult(result);
    }
}
