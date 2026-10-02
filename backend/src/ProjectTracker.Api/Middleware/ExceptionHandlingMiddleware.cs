using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Api.Common;
using ProjectTracker.Domain.Exceptions;

namespace ProjectTracker.Api.Middleware;

/// <summary>Catches any exception that escapes a controller and converts it to the standard
/// response envelope with an appropriate status code — no stack trace ever reaches the client.
/// Domain/business-rule violations map to 4xx with their message; anything unexpected maps to
/// 500 with a generic message (the real detail goes to Serilog, tagged with the correlation id
/// already on the request).</summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleAsync(context, ex);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            EntityNotFoundException => (HttpStatusCode.NotFound, exception.Message),
            TenantAccessDeniedException => (HttpStatusCode.Forbidden, exception.Message),
            DomainException => (HttpStatusCode.UnprocessableEntity, exception.Message),
            ConcurrencyConflictException => (HttpStatusCode.Conflict, exception.Message),
            // A raw EF optimistic-concurrency failure is semantically a conflict (409), never a
            // server fault (500) — handlers that want a richer payload still throw the domain
            // ConcurrencyConflictException above; this is the safety net for the rest.
            DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "The record was modified by another request. Please retry."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception processing {Method} {Path}", context.Request.Method, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception ({StatusCode}) processing {Method} {Path}", (int)statusCode, context.Request.Method, context.Request.Path);
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse.FromResult(Shared.Results.Result.Fail(message));
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
