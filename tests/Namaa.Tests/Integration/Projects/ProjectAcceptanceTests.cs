using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Namaa.Application.Projects.Dtos;
using Namaa.Tests.Common;

namespace Namaa.Tests.Integration.Projects;

[Collection(AcceptanceTestCollection.Name)]
public sealed class ProjectAcceptanceTests(NamaaApiFixture fixture)
{
    [Fact]
    public async Task CreateProject_UsesAuthenticatedUserAsActor_AndWritesAudit()
    {
        using var client = await fixture.CreateAuthenticatedClientAsync();
        var projectCode = NamaaApiFixture.NewCode("PRJ");
        using var response = await client.PostAsJsonAsync("/api/projects", new
        {
            fixture.CustomerId,
            ProjectCode = projectCode,
            Name = "Created through API",
            BudgetAmount = 800m,
            SourceQuotationId = (long?)null,
            ProjectManagerId = (long?)null,
            Description = (string?)null,
            Location = (string?)null,
            StartDate = (DateTime?)null,
            EndDate = (DateTime?)null,
            CreatedByUserId = long.MaxValue
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var dto = (await response.Content.ReadFromJsonAsync<ProjectDto>())!;
        Assert.Equal(fixture.ActiveUserId, dto.CreatedByUserId);
        Assert.NotEqual(long.MaxValue, dto.CreatedByUserId);

        await fixture.WithDbContextAsync(async db =>
        {
            var persisted = await db.Projects.AsNoTracking().SingleAsync(project => project.Id == dto.Id);
            var audit = await db.AuditLogs.AsNoTracking().SingleAsync(log =>
                log.EntityType == "Project" &&
                log.Action == "Project.Created" &&
                log.EntityId == dto.Id);
            Assert.Equal(projectCode, persisted.ProjectCode);
            Assert.Equal(fixture.ActiveUserId, persisted.CreatedByUserId);
            Assert.Equal(fixture.ActiveUserId, audit.UserId);
        });
    }

    [Fact]
    public async Task UpdateProject_WithStaleRowVersion_Returns409AndDoesNotOverwriteOrAudit()
    {
        var project = await fixture.SeedProjectAsync("Original project");
        using var client = await fixture.CreateAuthenticatedClientAsync();

        var original = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{project.Id}");
        var firstUpdate = new
        {
            Name = "Accepted project update",
            BudgetAmount = original!.BudgetAmount,
            original.Description,
            original.Location,
            original.StartDate,
            original.EndDate,
            original.ProjectManagerId,
            original.RowVersion
        };
        using var acceptedResponse = await client.PutAsJsonAsync($"/api/projects/{project.Id}", firstUpdate);
        Assert.Equal(HttpStatusCode.OK, acceptedResponse.StatusCode);
        var accepted = (await acceptedResponse.Content.ReadFromJsonAsync<ProjectDto>())!;
        Assert.NotEqual(original.RowVersion, accepted.RowVersion);

        using var staleResponse = await client.PutAsJsonAsync($"/api/projects/{project.Id}", new
        {
            Name = "Rejected stale project update",
            BudgetAmount = original.BudgetAmount,
            original.Description,
            original.Location,
            original.StartDate,
            original.EndDate,
            original.ProjectManagerId,
            original.RowVersion
        });
        Assert.Equal(HttpStatusCode.Conflict, staleResponse.StatusCode);

        var current = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{project.Id}");
        Assert.Equal("Accepted project update", current!.Name);
        Assert.Equal(accepted.RowVersion, current.RowVersion);
        var auditCount = await fixture.WithDbContextAsync(db => db.AuditLogs.CountAsync(log =>
            log.EntityType == "Project" &&
            log.Action == "Project.Updated" &&
            log.EntityId == project.Id));
        Assert.Equal(1, auditCount);
    }

    [Fact]
    public async Task CreateProject_WithMissingName_Returns400WithoutMutationOrAudit()
    {
        using var client = await fixture.CreateAuthenticatedClientAsync();
        var code = NamaaApiFixture.NewCode("BAD");
        var before = await fixture.WithDbContextAsync(db => db.AuditLogs.CountAsync());

        using var response = await client.PostAsJsonAsync("/api/projects", new
        {
            fixture.CustomerId,
            ProjectCode = code,
            Name = "",
            BudgetAmount = 10m
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await fixture.WithDbContextAsync(async db =>
        {
            Assert.False(await db.Projects.AnyAsync(project => project.ProjectCode == code));
            Assert.Equal(before, await db.AuditLogs.CountAsync());
        });
    }

    [Fact]
    public async Task UpdateProject_WithInvalidRowVersionLengths_Returns400WithoutMutationOrAudit()
    {
        var project = await fixture.SeedProjectAsync("RowVersion baseline");
        using var client = await fixture.CreateAuthenticatedClientAsync();
        var invalidTokens = new[]
        {
            "not-base64",
            Convert.ToBase64String(new byte[1]),
            Convert.ToBase64String(new byte[7]),
            Convert.ToBase64String(new byte[9])
        };

        foreach (var token in invalidTokens)
        {
            using var response = await client.PutAsJsonAsync($"/api/projects/{project.Id}", new
            {
                Name = "Must not persist",
                BudgetAmount = 1m,
                Description = (string?)null,
                Location = (string?)null,
                StartDate = (DateTime?)null,
                EndDate = (DateTime?)null,
                ProjectManagerId = (long?)null,
                RowVersion = token
            });
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        await fixture.WithDbContextAsync(async db =>
        {
            var current = await db.Projects.AsNoTracking().SingleAsync(item => item.Id == project.Id);
            Assert.Equal("RowVersion baseline", current.Name);
            Assert.False(await db.AuditLogs.AnyAsync(log =>
                log.EntityId == project.Id && log.Action == "Project.Updated"));
        });
    }
}
