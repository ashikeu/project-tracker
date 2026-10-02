using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Shared.Results;

namespace ProjectTracker.Api.Common;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Maps a Result/Result&lt;T&gt; to the appropriate HTTP status + envelope:
    /// validation failure → 422, generic failure → 400, success → 200 (caller can override
    /// with a more specific 201/204 where relevant).</summary>
    protected IActionResult FromResult<T>(Result<T> result)
    {
        if (result.Success) return Ok(ApiResponse<T>.FromResult(result));
        if (result.Errors.Count > 0) return UnprocessableEntity(ApiResponse<T>.FromResult(result));
        return BadRequest(ApiResponse<T>.FromResult(result));
    }

    protected IActionResult FromResult(Result result)
    {
        if (result.Success) return Ok(ApiResponse.FromResult(result));
        if (result.Errors.Count > 0) return UnprocessableEntity(ApiResponse.FromResult(result));
        return BadRequest(ApiResponse.FromResult(result));
    }
}
