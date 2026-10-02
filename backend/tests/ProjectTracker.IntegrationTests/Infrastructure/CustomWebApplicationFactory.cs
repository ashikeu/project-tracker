using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ProjectTracker.IntegrationTests.Infrastructure;

/// <summary>Boots the real API against a dedicated test database under the "Testing" environment
/// (which runs the dev seed so tests have known users, and skips rate limiting). The full stack is
/// exercised on real SQL Server — the only faithful way to test the tenant global query filter,
/// rowversion concurrency, cascade rules and the sync outbox.</summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string TestConnectionString =
        "Server=localhost;Database=ProjectTracker_Test;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", TestConnectionString);
        builder.UseSetting("Jwt:SigningKey", "integration-tests-signing-key-at-least-32-characters-long-xyz");
    }
}
