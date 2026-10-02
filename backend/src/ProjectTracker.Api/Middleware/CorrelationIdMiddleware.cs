using Serilog.Context;

namespace ProjectTracker.Api.Middleware;

/// <summary>Ensures every request carries a correlation id (reused from the
/// X-Correlation-Id request header if the caller supplied one, e.g. a gateway or the Flutter
/// client) and pushes it into the Serilog LogContext so every log line for this request can be
/// traced together, plus echoes it back on the response.</summary>
public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var existing) && !string.IsNullOrWhiteSpace(existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString("N");

        context.Response.Headers[HeaderName] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
