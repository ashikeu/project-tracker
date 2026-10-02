using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ProjectTracker.IntegrationTests.Infrastructure;

namespace ProjectTracker.IntegrationTests;

public class AuthTests : IntegrationTestBase
{
    public AuthTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task Login_with_seeded_user_returns_a_token()
    {
        using var client = NewClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = PmEmail, password = DevPassword });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await ReadEnvelopeAsync(response);
        Assert.True(body.GetProperty("success").GetBoolean());
        Assert.False(string.IsNullOrWhiteSpace(body.GetProperty("data").GetProperty("accessToken").GetString()));
    }

    [Fact]
    public async Task Login_with_wrong_password_is_rejected()
    {
        using var client = NewClient();
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email = PmEmail, password = "wrong-password" });

        Assert.False(response.IsSuccessStatusCode);
        var body = await ReadEnvelopeAsync(response);
        Assert.False(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task Register_new_organization_then_login_succeeds()
    {
        using var client = NewClient();
        var email = $"founder-{Guid.NewGuid():N}@example.test";

        var register = await client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            email,
            password = "Str0ng!Pass",
            firstName = "Fenn",
            lastName = "Founder",
            organizationName = "New Org " + Guid.NewGuid().ToString("N")[..6]
        });
        Assert.True(register.IsSuccessStatusCode, await register.Content.ReadAsStringAsync());

        var login = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password = "Str0ng!Pass" });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var body = await ReadEnvelopeAsync(login);
        Assert.True(body.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task Protected_endpoint_without_token_is_unauthorized()
    {
        using var client = NewClient();
        var response = await client.GetAsync("/api/v1/notifications");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Concurrent_logins_for_the_same_user_all_succeed()
    {
        // Regression: updating User.LastLoginAt on the rowversioned entity used to make parallel
        // logins collide on the optimistic-concurrency token — the first won and the rest threw
        // DbUpdateConcurrencyException, surfacing as 500. LastLoginAt is now a separate best-effort
        // save whose concurrency race is swallowed, so every concurrent login must still succeed.
        using var client = NewClient();

        var tasks = Enumerable.Range(0, 8).Select(_ =>
            client.PostAsJsonAsync("/api/v1/auth/login", new { email = PmEmail, password = DevPassword }));
        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
        foreach (var r in responses) r.Dispose();
    }
}
