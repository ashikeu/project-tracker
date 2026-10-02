using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ProjectTracker.IntegrationTests.Infrastructure;

namespace ProjectTracker.IntegrationTests;

public class TenantIsolationTests : IntegrationTestBase
{
    public TenantIsolationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task A_user_cannot_read_another_organizations_project()
    {
        // Org A (seeded Acme): PM creates a project.
        var clientA = await AuthedClientAsync(PmEmail);
        var createResponse = await clientA.PostAsJsonAsync("/api/v1/projects", new
        {
            key = ("A" + Guid.NewGuid().ToString("N")[..5]).ToUpperInvariant(),
            name = "Org A Secret Project",
            description = (string?)null,
            managerUserId = (Guid?)null,
            priority = "Medium",
            startDate = (DateOnly?)null,
            targetDate = (DateOnly?)null
        });
        Assert.True(createResponse.IsSuccessStatusCode, await createResponse.Content.ReadAsStringAsync());
        var created = await ReadEnvelopeAsync(createResponse);
        var projectId = created.GetProperty("data").GetProperty("id").GetString();

        // Org B: brand-new organization + user.
        var clientB = await RegisterAndLoginNewOrgAsync();

        // Direct fetch of org A's project from org B must not surface it.
        var fetch = await clientB.GetAsync($"/api/v1/projects/{projectId}");
        var fetchBody = await ReadEnvelopeAsync(fetch);
        Assert.False(fetchBody.GetProperty("success").GetBoolean());

        // And it must not appear in org B's project list.
        var list = await clientB.GetAsync("/api/v1/projects?page=1&pageSize=100");
        var listBody = await ReadEnvelopeAsync(list);
        var items = listBody.GetProperty("data").GetProperty("items");
        foreach (var item in items.EnumerateArray())
        {
            Assert.NotEqual(projectId, item.GetProperty("id").GetString());
        }
    }

    private async Task<HttpClient> RegisterAndLoginNewOrgAsync()
    {
        using var anon = NewClient();
        var email = $"orgb-{Guid.NewGuid():N}@example.test";
        const string password = "Str0ng!Pass";
        var register = await anon.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email,
            password,
            firstName = "Bea",
            lastName = "Betaorg",
            organizationName = "Org B " + Guid.NewGuid().ToString("N")[..6]
        });
        Assert.True(register.IsSuccessStatusCode, await register.Content.ReadAsStringAsync());

        var token = await LoginTokenAsync(email, password);
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
