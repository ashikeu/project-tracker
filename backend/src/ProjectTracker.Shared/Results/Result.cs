namespace ProjectTracker.Shared.Results;

public class ValidationError
{
    public string Field { get; }
    public string Message { get; }

    public ValidationError(string field, string message)
    {
        Field = field;
        Message = message;
    }
}

public class Result
{
    public bool Success { get; }
    public string? Message { get; }
    public IReadOnlyList<ValidationError> Errors { get; }

    protected Result(bool success, string? message, IReadOnlyList<ValidationError>? errors)
    {
        Success = success;
        Message = message;
        Errors = errors ?? Array.Empty<ValidationError>();
    }

    public static Result Ok(string? message = null) => new(true, message, null);

    public static Result Fail(string message) => new(false, message, null);

    public static Result FailValidation(IReadOnlyList<ValidationError> errors) =>
        new(false, "Validation failed", errors);
}

public class Result<T> : Result
{
    public T? Data { get; }

    private Result(bool success, T? data, string? message, IReadOnlyList<ValidationError>? errors)
        : base(success, message, errors)
    {
        Data = data;
    }

    public static Result<T> Ok(T data, string? message = null) => new(true, data, message, null);

    public static new Result<T> Fail(string message) => new(false, default, message, null);

    public static new Result<T> FailValidation(IReadOnlyList<ValidationError> errors) =>
        new(false, default, "Validation failed", errors);
}
