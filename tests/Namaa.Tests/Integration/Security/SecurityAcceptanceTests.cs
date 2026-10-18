using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Tests.Common;

namespace Namaa.Tests.Integration.Security;

[Collection(AcceptanceTestCollection.Name)]
public sealed class SecurityAcceptanceTests(NamaaApiFixture fixture)
{
    public static TheoryData<string> AuthenticatedOnlyEndpoints => new()
    {
        "/api/projects",
        "/api/accounts",
        "/api/audit-logs"
    };

    [Theory]
    [MemberData(nameof(AuthenticatedOnlyEndpoints))]
    public async Task CurrentPolicies_RejectAnonymousAndAllowAuthenticatedUser(string path)
    {
        using var anonymous = fixture.CreateClient();
        using var anonymousResponse = await anonymous.GetAsync(path);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        using var authenticated = await fixture.CreateAuthenticatedClientAsync();
        using var authenticatedResponse = await authenticated.GetAsync(path);
        Assert.Equal(HttpStatusCode.OK, authenticatedResponse.StatusCode);
    }

    [Fact]
    public async Task ProjectTask_FromAnotherParent_Returns404()
    {
        var projectA = await fixture.SeedProjectAsync("Project A");
        var projectB = await fixture.SeedProjectAsync("Project B");
        var taskFromB = await fixture.SeedProjectTaskAsync(projectB.Id);
        using var client = await fixture.CreateAuthenticatedClientAsync();

        using var response = await client.GetAsync($"/api/projects/{projectA.Id}/tasks/{taskFromB.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProjectFile_MetadataRequiresAuthentication_AndDoesNotExposeStorageKey()
    {
        var project = await fixture.SeedProjectAsync();
        var storageKey = $"private/{Guid.NewGuid():N}";
        var file = await fixture.SeedProjectFileAsync(project.Id, storageKey);

        using var anonymous = fixture.CreateClient();
        using var anonymousResponse = await anonymous.GetAsync($"/api/projects/{project.Id}/files/{file.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        using var authenticated = await fixture.CreateAuthenticatedClientAsync();
        using var response = await authenticated.GetAsync($"/api/projects/{project.Id}/files/{file.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var dto = await response.Content.ReadFromJsonAsync<ProjectFileDto>();
        Assert.Equal(file.Id, dto!.Id);
        Assert.False(body.Contains(storageKey, StringComparison.Ordinal));
        Assert.DoesNotContain("storageKey", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task AuditLogs_AreReadOnlyOverHttp()
    {
        using var client = await fixture.CreateAuthenticatedClientAsync();
        using var get = await client.GetAsync("/api/audit-logs");
        using var post = await client.PostAsJsonAsync("/api/audit-logs", new { });
        using var put = await client.PutAsJsonAsync("/api/audit-logs/1", new { });
        using var delete = await client.DeleteAsync("/api/audit-logs/1");

        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, post.StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, put.StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, delete.StatusCode);
    }

    [Fact]
    public async Task SerilogRequestLogging_DoesNotCaptureSecrets()
    {
        fixture.LogSink.Clear();
        using var client = fixture.CreateClient();
        var tokens = await fixture.LoginAsync(client);
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens.AccessToken);
        using var response = await client.GetAsync("/api/projects");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var logText = string.Join('\n', fixture.LogSink.Events.Select(logEvent =>
            string.Concat(
                logEvent.RenderMessage(CultureInfo.InvariantCulture),
                " ",
                string.Join(' ', logEvent.Properties.Values.Select(value => value.ToString())))));
        var forbiddenValues = new[]
        {
            fixture.Password,
            tokens.AccessToken,
            tokens.RefreshToken,
            fixture.SigningKey,
            fixture.DatabaseConnectionString
        };

        Assert.NotEmpty(fixture.LogSink.Events);
        Assert.DoesNotContain(fixture.LogSink.Events, logEvent =>
            logEvent.Properties.Keys.Any(key =>
                string.Equals(key, "Authorization", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(key, "AuthorizationHeader", StringComparison.OrdinalIgnoreCase)));
        Assert.All(forbiddenValues, secret =>
            Assert.False(logText.Contains(secret, StringComparison.Ordinal)));
    }
}
