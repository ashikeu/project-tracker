using System.Net.Http.Json;
using System.Text.Json;
using ProjectTracker.IntegrationTests.Infrastructure;

namespace ProjectTracker.IntegrationTests;

public class SyncTests : IntegrationTestBase
{
    public SyncTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Push_create_is_applied_assigns_taskkey_and_is_idempotent()
    {
        var client = await AuthedClientAsync(PmEmail);
        var projectId = await CreateProjectAsync(client);

        var taskId = Guid.NewGuid();
        var operationId = Guid.NewGuid();
        var op = BuildCreateOp(operationId, taskId, projectId, "Offline task");

        // First push -> Applied with a server-assigned TaskKey.
        var first = await PushAsync(client, op);
        var firstResult = first.GetProperty("data").GetProperty("results")[0];
        Assert.Equal("Applied", firstResult.GetProperty("status").GetString());
        var taskKey = firstResult.GetProperty("taskKey").GetString();
        Assert.False(string.IsNullOrWhiteSpace(taskKey));

        // Re-push the same OperationId -> identical result, no duplicate row/key.
        var second = await PushAsync(client, op);
        var secondResult = second.GetProperty("data").GetProperty("results")[0];
        Assert.Equal("Applied", secondResult.GetProperty("status").GetString());
        Assert.Equal(taskKey, secondResult.GetProperty("taskKey").GetString());
    }

    [Fact]
    public async Task Push_update_with_stale_rowversion_is_a_conflict()
    {
        var client = await AuthedClientAsync(PmEmail);
        var projectId = await CreateProjectAsync(client);

        var taskId = Guid.NewGuid();
        await PushAsync(client, BuildCreateOp(Guid.NewGuid(), taskId, projectId, "Conflict task"));

        // Update with a bogus base row version must not overwrite — it conflicts.
        var updateOp = new
        {
            operationId = Guid.NewGuid(),
            entityType = "Task",
            entityId = taskId,
            changeType = "Updated",
            payloadJson = BuildTaskPayloadJson(taskId, projectId, "Edited with stale version"),
            baseRowVersion = "AAAAAAAAAAA="
        };
        var response = await PushAsync(client, updateOp);
        var result = response.GetProperty("data").GetProperty("results")[0];
        Assert.Equal("Conflict", result.GetProperty("status").GetString());
        Assert.False(string.IsNullOrWhiteSpace(result.GetProperty("serverPayloadJson").GetString()));
    }

    [Fact]
    public async Task Pull_returns_changes_after_a_watermark()
    {
        var client = await AuthedClientAsync(PmEmail);
        var projectId = await CreateProjectAsync(client);

        var baseline = await client.GetAsync("/api/v1/sync/pull?since=0&pageSize=500");
        var watermark = (await ReadEnvelopeAsync(baseline)).GetProperty("data").GetProperty("nextVersion").GetInt64();

        var taskId = Guid.NewGuid();
        await PushAsync(client, BuildCreateOp(Guid.NewGuid(), taskId, projectId, "Pull task"));

        var delta = await client.GetAsync($"/api/v1/sync/pull?since={watermark}&pageSize=500");
        var changes = (await ReadEnvelopeAsync(delta)).GetProperty("data").GetProperty("changes");
        var found = false;
        foreach (var c in changes.EnumerateArray())
        {
            if (c.GetProperty("entityId").GetString() == taskId.ToString()) found = true;
        }
        Assert.True(found);
    }

    private static object BuildCreateOp(Guid operationId, Guid taskId, string projectId, string title) => new
    {
        operationId,
        entityType = "Task",
        entityId = taskId,
        changeType = "Created",
        payloadJson = BuildTaskPayloadJson(taskId, projectId, title),
        baseRowVersion = (string?)null
    };

    private static string BuildTaskPayloadJson(Guid taskId, string projectId, string title) => JsonSerializer.Serialize(new
    {
        id = taskId,
        taskKey = "",
        projectId,
        title,
        description = (string?)null,
        taskType = 0,
        status = 0,
        priority = 2,
        reporterUserId = taskId,
        loggedHours = 0,
        position = 1000.0,
        isDeleted = false,
        rowVersion = ""
    });

    private async Task<JsonElement> PushAsync(HttpClient client, object op)
    {
        var response = await client.PostAsJsonAsync("/api/v1/sync/push", new { operations = new[] { op } });
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        return await ReadEnvelopeAsync(response);
    }

    private async Task<string> CreateProjectAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/v1/projects", new
        {
            key = ("Y" + Guid.NewGuid().ToString("N")[..5]).ToUpperInvariant(),
            name = "Sync Test Project",
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
