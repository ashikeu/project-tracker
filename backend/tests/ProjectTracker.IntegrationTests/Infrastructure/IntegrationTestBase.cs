using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ProjectTracker.IntegrationTests.Infrastructure;

[Collection("Integration")]
public abstract class IntegrationTestBase
{
    // Dev-seeded users (see DevDataSeeder) — password is shared across all of them.
    protected const string DevPassword = "Passw0rd!";
    protected const string OwnerEmail = "owner@acme.test";
    protected const string PmEmail = "pm@acme.test";
    protected const string DevEmail = "dev@acme.test";
    protected const string TesterEmail = "tester@acme.test";

    protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    protected readonly CustomWebApplicationFactory Factory;

    protected IntegrationTestBase(CustomWebApplicationFactory factory) => Factory = factory;

    protected HttpClient NewClient() => Factory.CreateClient();

    protected async Task<HttpClient> AuthedClientAsync(string email, string password = DevPassword)
    {
        var token = await LoginTokenAsync(email, password);
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected async Task<string> LoginTokenAsync(string email, string password = DevPassword)
    {
        using var client = Factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        var doc = await response.Content.ReadFromJsonAsync<JsonElement>();
        return doc.GetProperty("data").GetProperty("accessToken").GetString()!;
    }

    protected static async Task<JsonElement> ReadEnvelopeAsync(HttpResponseMessage response)
        => await response.Content.ReadFromJsonAsync<JsonElement>();

    protected static string NewSlug(string prefix) => $"{prefix}-{Guid.NewGuid():N}".Substring(0, 20);
}
