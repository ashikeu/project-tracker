using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Api.Extensions;
using ProjectTracker.Api.Filters;
using ProjectTracker.Api.Middleware;
using ProjectTracker.Application;
using ProjectTracker.Application.Common.Interfaces;
using ProjectTracker.Infrastructure;
using ProjectTracker.Infrastructure.Persistence;
using ProjectTracker.Infrastructure.Persistence.Seed;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName());

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationActionFilter>();
    })
    .AddJsonOptions(options =>
    {
        // Enums travel as their string names (e.g. "High", not 3) on the wire — matches every
        // DTO here that already renders enums via ToString().
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddApiVersioningSetup();
    builder.Services.AddSwaggerWithJwt();
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddPermissionAuthorization();
    builder.Services.AddAppHealthChecks(builder.Configuration);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Default", policy =>
        {
            var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
            if (origins.Length > 0)
            {
                policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            }
            else if (builder.Environment.IsDevelopment())
            {
                // Dev convenience only: reflect any origin so Swagger/local clients work. Never the
                // fallback in a deployed environment — there, an empty Cors:AllowedOrigins means
                // no cross-origin access rather than "allow everyone".
                policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => true);
            }
        });
    });

    // Rate limiting: a per-IP global cap plus a stricter window on the auth endpoints (credential
    // stuffing / brute-force defense). Limits are generous enough not to impede a normal client.
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions { Window = TimeSpan.FromMinutes(1), PermitLimit = 600, QueueLimit = 0 }));

        options.AddPolicy("auth", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions { Window = TimeSpan.FromMinutes(1), PermitLimit = 30, QueueLimit = 0 }));
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Retry the initial migrate: in a containerized run the SQL Server may still be finishing
        // its first-boot even after its healthcheck passes, so a transient connect failure here is
        // expected and recoverable rather than fatal.
        const int maxAttempts = 12;
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                await db.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                Log.Warning(ex, "Database not ready (attempt {Attempt}/{Max}); retrying in 5s.", attempt, maxAttempts);
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        await RbacSeeder.SeedAsync(db);

        // Dev seed also runs under the integration-test host so tests have known users to auth as.
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing"))
        {
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            await DevDataSeeder.SeedAsync(db, passwordHasher);
        }
    }

    app.UseSerilogRequestLogging();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Baseline security response headers on every response.
    app.Use(async (context, next) =>
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "no-referrer";
        headers["X-Permitted-Cross-Domain-Policies"] = "none";
        await next();
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Tracker API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseCors("Default");

    // Rate limiting is skipped under the test host so the suite's rapid logins aren't throttled.
    if (!app.Environment.IsEnvironment("Testing"))
    {
        app.UseRateLimiter();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthChecks("/health");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Project Tracker API host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>Exposes Program for WebApplicationFactory-based integration tests.</summary>
public partial class Program { }
