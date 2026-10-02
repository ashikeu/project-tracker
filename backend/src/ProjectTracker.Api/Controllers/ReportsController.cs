using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Reports.Queries.GetCompletionReport;
using ProjectTracker.Application.Features.Reports.Queries.GetEpicProgressReport;
using ProjectTracker.Application.Features.Reports.Queries.GetSprintReport;
using ProjectTracker.Application.Features.Reports.Queries.GetTeamProductivityReport;
using ProjectTracker.Application.Features.Reports.Queries.GetTimeTrackingReport;
using ProjectTracker.Application.Features.Reports.Queries.GetWorkloadReport;
using ProjectTracker.Shared.Constants;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reports")]
[Authorize]
public class ReportsController : ApiControllerBase
{
    [HttpGet("sprint/{sprintId:guid}")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> SprintReport(Guid sprintId, [FromServices] IGetSprintReportHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetSprintReportQuery(sprintId), ct);
        return FromResult(result);
    }

    [HttpGet("workload")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> WorkloadReport([FromQuery] Guid projectId, [FromServices] IGetWorkloadReportHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetWorkloadReportQuery(projectId), ct);
        return FromResult(result);
    }

    [HttpGet("epic-progress")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> EpicProgressReport([FromQuery] Guid projectId, [FromServices] IGetEpicProgressReportHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetEpicProgressReportQuery(projectId), ct);
        return FromResult(result);
    }

    [HttpGet("time-tracking")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> TimeTrackingReport([FromQuery] Guid projectId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromServices] IGetTimeTrackingReportHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetTimeTrackingReportQuery(projectId, from, to), ct);
        return FromResult(result);
    }

    [HttpGet("team-productivity")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> TeamProductivityReport([FromQuery] Guid projectId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromServices] IGetTeamProductivityReportHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetTeamProductivityReportQuery(projectId, from, to), ct);
        return FromResult(result);
    }

    [HttpGet("completion")]
    [Authorize(Policy = PermissionKeys.ProjectView)]
    public async Task<IActionResult> CompletionReport([FromQuery] Guid projectId, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromServices] IGetCompletionReportHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetCompletionReportQuery(projectId, from, to), ct);
        return FromResult(result);
    }
}
