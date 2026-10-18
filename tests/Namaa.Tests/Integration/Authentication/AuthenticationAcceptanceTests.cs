using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Namaa.Application.Authentication.Dtos;
using Namaa.Tests.Common;

namespace Namaa.Tests.Integration.Authentication;

[Collection(AcceptanceTestCollection.Name)]
public sealed class AuthenticationAcceptanceTests(NamaaApiFixture fixture)
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokensWithoutPasswordHash()
    {
        using var client = fixture.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = fixture.ActiveEmail,
            fixture.Password
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var tokens = JsonSerializer.Deserialize<AuthTokenDto>(body, JsonOptions)!;
        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
        Assert.DoesNotContain("passwordHash", body, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Login_WithWrongOrUnknownCredentials_ReturnsSameGeneric401()
    {
        using var client = fixture.CreateClient();
        using var wrongPassword = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = fixture.ActiveEmail,
            Password = fixture.Password + "-wrong"
        });
        using var unknownEmail = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = $"unknown-{Guid.NewGuid():N}@example.test",
            fixture.Password
        });

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassword.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unknownEmail.StatusCode);
        Assert.Equal(
            await ReadProblemCodeAsync(wrongPassword),
            await ReadProblemCodeAsync(unknownEmail));
    }

    [Fact]
    public async Task Login_WithInactiveUser_Returns401()
    {
        using var client = fixture.CreateClient();
        using var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = fixture.InactiveEmail,
            fixture.Password
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Authentication.UserInactive", await ReadProblemCodeAsync(response));
    }

    [Fact]
    public async Task Refresh_RotatesToken_AndDatabaseStoresOnlyHashes()
    {
        using var client = fixture.CreateClient();
        var first = await fixture.LoginAsync(client);

        var storedHash = await fixture.WithDbContextAsync(db => db.RefreshTokens
            .AsNoTracking()
            .Where(token => token.UserId == fixture.ActiveUserId)
            .OrderByDescending(token => token.Id)
            .Select(token => token.TokenHash)
            .FirstAsync());
        var expectedHash = Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(first.RefreshToken)));

        Assert.NotEqual(first.RefreshToken, storedHash);
        Assert.Equal(expectedHash, storedHash);

        using var rotate = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            first.RefreshToken
        });
        Assert.Equal(HttpStatusCode.OK, rotate.StatusCode);
        var second = (await rotate.Content.ReadFromJsonAsync<AuthTokenDto>())!;
        Assert.NotEqual(first.RefreshToken, second.RefreshToken);

        using var oldRetry = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            first.RefreshToken
        });
        Assert.Equal(HttpStatusCode.Unauthorized, oldRetry.StatusCode);

        using var newRetry = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            second.RefreshToken
        });
        Assert.Equal(HttpStatusCode.OK, newRetry.StatusCode);
    }

    [Fact]
    public async Task Logout_RevokesRefreshToken()
    {
        using var client = fixture.CreateClient();
        var tokens = await fixture.LoginAsync(client);

        using var logout = await client.PostAsJsonAsync("/api/auth/logout", new
        {
            tokens.RefreshToken
        });
        Assert.Equal(HttpStatusCode.OK, logout.StatusCode);

        using var refresh = await client.PostAsJsonAsync("/api/auth/refresh", new
        {
            tokens.RefreshToken
        });
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    private static async Task<string> ReadProblemCodeAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("code").GetString()!;
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
}
