namespace ProjectTracker.IntegrationTests.Infrastructure;

/// <summary>All integration tests share one app/DB and run serially (no parallelization across the
/// collection), so they don't race on the shared test database.</summary>
[CollectionDefinition("Integration", DisableParallelization = true)]
public class IntegrationCollection : ICollectionFixture<CustomWebApplicationFactory>
{
}
