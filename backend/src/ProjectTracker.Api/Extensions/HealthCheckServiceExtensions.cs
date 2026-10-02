namespace ProjectTracker.Api.Extensions;

public static class HealthCheckServiceExtensions
{
    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddSqlServer(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "sql-server",
                tags: new[] { "db", "ready" });

        return services;
    }
}
