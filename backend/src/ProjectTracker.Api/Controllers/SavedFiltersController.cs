using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.SavedFilters.Commands.CreateSavedFilter;
using ProjectTracker.Application.Features.SavedFilters.Commands.DeleteSavedFilter;
using ProjectTracker.Application.Features.SavedFilters.Queries.GetSavedFilters;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/saved-filters")]
[Authorize]
public class SavedFiltersController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSavedFilters([FromQuery] string? scope, [FromQuery] Guid? projectId, [FromServices] IGetSavedFiltersHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new GetSavedFiltersQuery(scope, projectId), ct);
        return Ok(ApiResponse<object>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSavedFilter([FromBody] CreateSavedFilterRequest request, [FromServices] ICreateSavedFilterHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new CreateSavedFilterCommand(request.Name, request.Scope, request.ProjectId, request.FilterJson), ct);
        return FromResult(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSavedFilter(Guid id, [FromServices] IDeleteSavedFilterHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(new DeleteSavedFilterCommand(id), ct);
        return FromResult(result);
    }
}

public record CreateSavedFilterRequest(string Name, string Scope, Guid? ProjectId, string FilterJson);
