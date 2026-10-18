using System.Net;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Namaa.Application.Common.Authorization;
using Namaa.Application.JournalEntries.Dtos;
using Namaa.Domain.Entities;
using Namaa.Tests.Common;

namespace Namaa.Tests.Integration.Security;

[Collection(AcceptanceTestCollection.Name)]
public sealed class RoleAuthorizationAcceptanceTests(NamaaApiFixture fixture)
{
    [Fact]
    public async Task Administrator_CanAccessEveryModulePolicy()
    {
        var endpoints = new[]
        {
            "/api/projects",
            "/api/customers",
            "/api/accounts",
            "/api/users",
            "/api/roles",
            "/api/suppliers",
            "/api/notifications",
            "/api/audit-logs"
        };
        using var client = await fixture.CreateAuthenticatedClientAsync();

        foreach (var endpoint in endpoints)
        {
            using var response = await client.GetAsync(endpoint);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task ProjectManager_CanManageProjectsButCannotAccessFinance()
    {
        var session = await fixture.CreateAuthenticatedClientForRoleAsync(RoleNames.ProjectManager);
        using var client = session.Client;

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(
            client.DefaultRequestHeaders.Authorization!.Parameter!);

        using var projects = await client.GetAsync("/api/projects");
        using var finance = await client.GetAsync("/api/accounts");

        Assert.Contains(jwt.Claims, claim =>
            claim.Value == RoleNames.ProjectManager &&
            claim.Type is ClaimTypes.Role or "role");
        Assert.Equal(HttpStatusCode.OK, projects.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, finance.StatusCode);
    }

    [Fact]
    public async Task Sales_CanAccessCrmButCannotAccessHumanResources()
    {
        var session = await fixture.CreateAuthenticatedClientForRoleAsync(RoleNames.Sales);
        using var client = session.Client;

        using var customers = await client.GetAsync("/api/customers");
        using var users = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, customers.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, users.StatusCode);
    }

    [Fact]
    public async Task Accountant_CanManageFinanceAndPostJournalEntry()
    {
        var account = await fixture.SeedAccountAsync();
        var session = await fixture.CreateAuthenticatedClientForRoleAsync(RoleNames.Accountant);
        using var client = session.Client;
        var journal = await CreateJournalAsync(client);
        var debit = await AddLineAsync(client, journal.Id, account.Id, 75m, 0m, journal.RowVersion);
        var credit = await AddLineAsync(
            client,
            journal.Id,
            account.Id,
            0m,
            75m,
            debit.ParentRowVersion);

        using var response = await client.PostAsJsonAsync(
            $"/api/journal-entries/{journal.Id}/post",
            new { PostedAt = DateTime.UtcNow, RowVersion = credit.ParentRowVersion });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await fixture.WithDbContextAsync(async dbContext =>
        {
            Assert.True(await dbContext.AuditLogs.AnyAsync(audit =>
                audit.Action == "JournalEntry.Posted" &&
                audit.EntityId == journal.Id &&
                audit.UserId == session.UserId));
        });
    }

    [Fact]
    public async Task HumanResources_CanManageUsersButCannotManageRoles()
    {
        var session = await fixture.CreateAuthenticatedClientForRoleAsync(RoleNames.HR);
        using var client = session.Client;

        using var users = await client.GetAsync("/api/users");
        using var roles = await client.GetAsync("/api/roles");

        Assert.Equal(HttpStatusCode.OK, users.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, roles.StatusCode);
    }

    [Fact]
    public async Task Procurement_CanAccessProcurementButCannotAccessFinance()
    {
        var session = await fixture.CreateAuthenticatedClientForRoleAsync(RoleNames.Procurement);
        using var client = session.Client;

        using var suppliers = await client.GetAsync("/api/suppliers");
        using var finance = await client.GetAsync("/api/accounts");

        Assert.Equal(HttpStatusCode.OK, suppliers.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, finance.StatusCode);
    }

    [Fact]
    public async Task Employee_IsForbiddenFromManagementWhileAnonymousIsUnauthorized()
    {
        var auditCount = await fixture.WithDbContextAsync(dbContext =>
            dbContext.AuditLogs.CountAsync(audit => audit.Action == "Project.Created"));
        var employeeSession = await fixture.CreateAuthenticatedClientForRoleAsync(RoleNames.Employee);
        using var employee = employeeSession.Client;
        using var denied = await employee.PostAsJsonAsync("/api/projects", new { });
        using var notifications = await employee.GetAsync("/api/notifications");
        using var anonymous = fixture.CreateClient();
        using var unauthenticated = await anonymous.GetAsync("/api/projects");

        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
        Assert.Equal(HttpStatusCode.OK, notifications.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticated.StatusCode);
        Assert.Equal(auditCount, await fixture.WithDbContextAsync(dbContext =>
            dbContext.AuditLogs.CountAsync(audit => audit.Action == "Project.Created")));
    }

    [Fact]
    public async Task Administrator_CannotBypassNotificationOwnership()
    {
        var employeeSession = await fixture.CreateAuthenticatedClientForRoleAsync(RoleNames.Employee);
        using var employee = employeeSession.Client;
        var notification = await fixture.WithDbContextAsync(async dbContext =>
        {
            var entity = Notification.Create(
                employeeSession.UserId,
                "Private notification",
                "Recipient-only notification.");
            dbContext.Notifications.Add(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        });
        using var administrator = await fixture.CreateAuthenticatedClientAsync();

        using var administratorResponse = await administrator.GetAsync(
            $"/api/notifications/{notification.Id}");
        using var employeeResponse = await employee.GetAsync(
            $"/api/notifications/{notification.Id}");

        Assert.Equal(HttpStatusCode.NotFound, administratorResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, employeeResponse.StatusCode);
    }

    [Fact]
    public async Task AuditLogs_AreAdministratorOnly()
    {
        using var administrator = await fixture.CreateAuthenticatedClientAsync();
        var accountantSession = await fixture.CreateAuthenticatedClientForRoleAsync(RoleNames.Accountant);
        using var accountant = accountantSession.Client;

        using var allowed = await administrator.GetAsync("/api/audit-logs");
        using var denied = await accountant.GetAsync("/api/audit-logs");

        Assert.Equal(HttpStatusCode.OK, allowed.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, denied.StatusCode);
    }

    private static async Task<JournalEntryDto> CreateJournalAsync(HttpClient client)
    {
        using var response = await client.PostAsJsonAsync("/api/journal-entries", new
        {
            EntryNumber = NamaaApiFixture.NewCode("JE"),
            EntryDate = DateTime.UtcNow.Date,
            Description = "Role authorization journal",
            SourceType = (string?)null,
            SourceId = (long?)null
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JournalEntryDto>())!;
    }

    private static async Task<JournalEntryLineDto> AddLineAsync(
        HttpClient client,
        long journalId,
        long accountId,
        decimal debit,
        decimal credit,
        string rowVersion)
    {
        using var response = await client.PostAsJsonAsync($"/api/journal-entries/{journalId}/lines", new
        {
            AccountId = accountId,
            Debit = debit,
            Credit = credit,
            BranchId = (long?)null,
            CostCenterId = (long?)null,
            ProjectId = (long?)null,
            Description = (string?)null,
            RowVersion = rowVersion
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<JournalEntryLineDto>())!;
    }
}
