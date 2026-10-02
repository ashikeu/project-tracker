using System.Net;
using System.Net.Http.Json;
using ProjectTracker.IntegrationTests.Infrastructure;

namespace ProjectTracker.IntegrationTests;

public class RbacTests : IntegrationTestBase
{
    public RbacTests(CustomWebApplicationFactory factory) : base(factory) { }

    private static object NewProjectBody() => new
    {
        key = ("K" + Guid.NewGuid().ToString("N")[..5]).ToUpperInvariant(),
        name = "RBAC Project",
        description = (string?)null,
        managerUserId = (Guid?)null,
        priority = "Medium",
        startDate = (DateOnly?)null,
        targetDate = (DateOnly?)null
    };

    [Fact]
    public async Task ProjectManager_can_create_a_project()
    {
        var client = await AuthedClientAsync(PmEmail);
        var response = await client.PostAsJsonAsync("/api/v1/projects", NewProjectBody());
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Developer_cannot_create_a_project()
    {
        var client = await AuthedClientAsync(DevEmail);
        var response = await client.PostAsJsonAsync("/api/v1/projects", NewProjectBody());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Tester_cannot_create_a_project()
    {
        var client = await AuthedClientAsync(TesterEmail);
        var response = await client.PostAsJsonAsync("/api/v1/projects", NewProjectBody());
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
