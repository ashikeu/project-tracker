using ProjectTracker.Shared.Results;

namespace ProjectTracker.Api.Common;

/// <summary>The response envelope every endpoint returns: { success, data, message, errors }.</summary>
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ApiValidationError> Errors { get; init; } = Array.Empty<ApiValidationError>();

    public static ApiResponse<T> Ok(T data, string? message = null) => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> FromResult(Result<T> result) => new()
    {
        Success = result.Success,
        Data = result.Data,
        Message = result.Message,
        Errors = result.Errors.Select(e => new ApiValidationError(e.Field, e.Message)).ToList()
    };
}

public class ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ApiValidationError> Errors { get; init; } = Array.Empty<ApiValidationError>();

    public static ApiResponse Ok(string? message = null) => new() { Success = true, Message = message };

    public static ApiResponse FromResult(Result result) => new()
    {
        Success = result.Success,
        Message = result.Message,
        Errors = result.Errors.Select(e => new ApiValidationError(e.Field, e.Message)).ToList()
    };
}

public record ApiValidationError(string Field, string Message);
