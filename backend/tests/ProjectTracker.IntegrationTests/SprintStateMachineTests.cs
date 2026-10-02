using System.Net.Http.Json;
using System.Text.Json;
using ProjectTracker.IntegrationTests.Infrastructure;

namespace ProjectTracker.IntegrationTests;

public class SprintStateMachineTests : IntegrationTestBase
{
    public SprintStateMachineTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Sprint_lifecycle_enforces_valid_transitions()
    {
        var client = await AuthedClientAsync(PmEmail);
        var projectId = await CreateProjectAsync(client);

        // Create a sprint (starts in Planning).
        var createResponse = await client.PostAsJsonAsync("/api/v1/sprints", new { projectId, name = "Sprint A" });
        Assert.True(createResponse.IsSuccessStatusCode, await createResponse.Content.ReadAsStringAsync());
        var sprintId = (await ReadEnvelopeAsync(createResponse)).GetProperty("data").GetProperty("id").GetString();

        // Completing a Planning sprint is invalid.
        var completeTooEarly = await client.PostAsJsonAsync($"/api/v1/sprints/{sprintId}/complete", new { moveIncompleteToBacklog = true });
        Assert.False((await ReadEnvelopeAsync(completeTooEarly)).GetProperty("success").GetBoolean());

        // Start it (Planning -> Active).
        var start = await client.PostAsync($"/api/v1/sprints/{sprintId}/start", null);
        Assert.True((await ReadEnvelopeAsync(start)).GetProperty("success").GetBoolean());

        // Starting again is invalid (already Active).
        var startAgain = await client.PostAsync($"/api/v1/sprints/{sprintId}/start", null);
        Assert.False((await ReadEnvelopeAsync(startAgain)).GetProperty("success").GetBoolean());

        // Completing an Active sprint succeeds.
        var complete = await client.PostAsJsonAsync($"/api/v1/sprints/{sprintId}/complete", new { moveIncompleteToBacklog = true });
        var completeBody = await ReadEnvelopeAsync(complete);
        Assert.True(completeBody.GetProperty("success").GetBoolean());
        Assert.Equal("Completed", completeBody.GetProperty("data").GetProperty("status").GetString());
    }

    [Fact]
    public async Task Only_one_active_sprint_per_project()
    {
        var client = await AuthedClientAsync(PmEmail);
        var projectId = await CreateProjectAsync(client);

        var first = (await ReadEnvelopeAsync(await client.PostAsJsonAsync("/api/v1/sprints", new { projectId, name = "First" })))
            .GetProperty("data").GetProperty("id").GetString();
        var second = (await ReadEnvelopeAsync(await client.PostAsJsonAsync("/api/v1/sprints", new { projectId, name = "Second" })))
            .GetProperty("data").GetProperty("id").GetString();

        Assert.True((await ReadEnvelopeAsync(await client.PostAsync($"/api/v1/sprints/{first}/start", null))).GetProperty("success").GetBoolean());
        // Second cannot start while the first is active.
        Assert.False((await ReadEnvelopeAsync(await client.PostAsync($"/api/v1/sprints/{second}/start", null))).GetProperty("success").GetBoolean());
    }

    private async Task<string> CreateProjectAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/projects", new
        {
            key = ("S" + Guid.NewGuid().ToString("N")[..5]).ToUpperInvariant(),
            name = "Sprint Test Project",
            description = (string?)null,
            managerUserId = (Guid?)null,
            priority = "Medium",
            startDate = (DateOnly?)null,
            targetDate = (DateOnly?)null
        });
        response.EnsureSuccessStatusCode();
        return (await ReadEnvelopeAsync(response)).GetProperty("data").GetProperty("id").GetString()!;
    }
}
