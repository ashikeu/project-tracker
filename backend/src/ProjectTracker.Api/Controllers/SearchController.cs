using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Api.Common;
using ProjectTracker.Application.Features.Search.Queries.GlobalSearch;

namespace ProjectTracker.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/search")]
[Authorize]
public class SearchController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] string? types, [FromServices] IGlobalSearchHandler handler, CancellationToken ct)
    {
        var typeSet = string.IsNullOrWhiteSpace(types)
            ? Array.Empty<string>()
            : types.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                   .Select(t => t.ToLowerInvariant()).ToArray();

        var result = await handler.Handle(new GlobalSearchQuery(q ?? string.Empty, typeSet), ct);
        return FromResult(result);
    }
}
