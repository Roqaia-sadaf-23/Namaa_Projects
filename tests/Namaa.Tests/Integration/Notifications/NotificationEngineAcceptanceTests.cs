using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Namaa.Application.Common.Auditing;
using Namaa.Application.Invoices.Dtos;
using Namaa.Application.Notifications.Dtos;
using Namaa.Application.Notifications.Events;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Entities;
using Namaa.Domain.Enums;
using Namaa.Tests.Common;

namespace Namaa.Tests.Integration.Notifications;

[Collection(AcceptanceTestCollection.Name)]
public sealed class NotificationEngineAcceptanceTests(NamaaApiFixture fixture)
{
    [Fact]
    public async Task TaskAssignment_CommitsTaskNotificationAndBusinessAuditTogether()
    {
        var project = await fixture.SeedProjectAsync();
        var sensitiveDescription = $"secret-{Guid.NewGuid():N}";
        using var client = await fixture.CreateAuthenticatedClientAsync();

        using var response = await client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/tasks",
            new
            {
                Title = "Prepare implementation plan",
                Priority = TaskPriority.Medium,
                Description = sensitiveDescription,
                StartDate = (DateTime?)null,
                DueDate = (DateTime?)null,
                AssignedToUserId = fixture.ActiveUserId,
                ParentTaskId = (long?)null,
                StageId = (long?)null,
                TaskType = "Technical"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = (await response.Content.ReadFromJsonAsync<ProjectTaskDto>())!;
        await fixture.WithDbContextAsync(async dbContext =>
        {
            Assert.True(await dbContext.ProjectTasks.AnyAsync(entity => entity.Id == task.Id));
            var notification = await dbContext.Notifications.SingleAsync(entity =>
                entity.UserId == fixture.ActiveUserId &&
                entity.EntityType == NotificationTypes.ProjectTaskAssigned &&
                entity.EntityId == task.Id);
            Assert.Contains(task.Title, notification.Message, StringComparison.Ordinal);
            Assert.DoesNotContain(sensitiveDescription, notification.Message, StringComparison.Ordinal);
            Assert.True(await dbContext.AuditLogs.AnyAsync(audit =>
                audit.Action == "ProjectTask.Created" && audit.EntityId == task.Id));
        });
    }

    [Fact]
    public async Task ProjectMemberAdded_CreatesNotificationForAddedMember()
    {
        var project = await fixture.SeedProjectAsync();
        var recipient = await fixture.SeedActiveUserAsync();
        using var client = await fixture.CreateAuthenticatedClientAsync();

        using var response = await client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/members",
            new
            {
                UserId = recipient.UserId,
                JoinedAt = DateTime.UtcNow,
                ProjectRole = "Contributor"
            });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var member = (await response.Content.ReadFromJsonAsync<ProjectMemberDto>())!;
        await fixture.WithDbContextAsync(async dbContext =>
        {
            var notification = await dbContext.Notifications.SingleAsync(entity =>
                entity.UserId == recipient.UserId &&
                entity.EntityType == NotificationTypes.ProjectMemberAdded &&
                entity.EntityId == member.Id);
            Assert.Equal("Project membership added", notification.Title);
        });
    }

    [Fact]
    public async Task InvoiceIssue_CreatesOneNotificationAndStaleRetryCreatesNone()
    {
        using var client = await fixture.CreateAuthenticatedClientAsync();
        var invoiceNumber = NamaaApiFixture.NewCode("INV");
        using var createResponse = await client.PostAsJsonAsync(
            "/api/invoices",
            new
            {
                InvoiceNumber = invoiceNumber,
                IssueDate = DateTime.UtcNow.Date,
                DueDate = DateTime.UtcNow.Date.AddDays(30),
                ProjectId = (long?)null,
                CustomerId = fixture.CustomerId,
                Notes = "Internal note must not be copied to notifications"
            });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = (await createResponse.Content.ReadFromJsonAsync<InvoiceDto>())!;

        using var issueResponse = await client.PostAsJsonAsync(
            $"/api/invoices/{created.Id}/issue",
            new { created.RowVersion });
        Assert.Equal(HttpStatusCode.OK, issueResponse.StatusCode);

        using var staleResponse = await client.PostAsJsonAsync(
            $"/api/invoices/{created.Id}/issue",
            new { created.RowVersion });
        Assert.Equal(HttpStatusCode.Conflict, staleResponse.StatusCode);

        await fixture.WithDbContextAsync(async dbContext =>
        {
            var notifications = await dbContext.Notifications
                .Where(entity =>
                    entity.UserId == fixture.ActiveUserId &&
                    entity.EntityType == NotificationTypes.InvoiceIssued &&
                    entity.EntityId == created.Id)
                .ToListAsync();
            Assert.Single(notifications);
            Assert.Contains(invoiceNumber, notifications[0].Message, StringComparison.Ordinal);
            Assert.DoesNotContain("Internal note", notifications[0].Message, StringComparison.Ordinal);
            Assert.Equal(1, await dbContext.AuditLogs.CountAsync(audit =>
                audit.Action == "Invoice.Issued" && audit.EntityId == created.Id));
        });
    }

    [Fact]
    public async Task FailedTaskAssignment_CreatesNeitherTaskNotificationNorSuccessAudit()
    {
        var project = await fixture.SeedProjectAsync();
        var notificationCount = await fixture.WithDbContextAsync(dbContext =>
            dbContext.Notifications.CountAsync(entity =>
                entity.EntityType == NotificationTypes.ProjectTaskAssigned));
        var auditCount = await fixture.WithDbContextAsync(dbContext =>
            dbContext.AuditLogs.CountAsync(audit => audit.Action == "ProjectTask.Created"));
        using var client = await fixture.CreateAuthenticatedClientAsync();

        using var response = await client.PostAsJsonAsync(
            $"/api/projects/{project.Id}/tasks",
            new
            {
                Title = "Invalid assignment",
                Priority = TaskPriority.Medium,
                Description = (string?)null,
                StartDate = (DateTime?)null,
                DueDate = (DateTime?)null,
                AssignedToUserId = long.MaxValue,
                ParentTaskId = (long?)null,
                StageId = (long?)null,
                TaskType = (string?)null
            });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        await fixture.WithDbContextAsync(async dbContext =>
        {
            Assert.Equal(notificationCount, await dbContext.Notifications.CountAsync(entity =>
                entity.EntityType == NotificationTypes.ProjectTaskAssigned));
            Assert.Equal(auditCount, await dbContext.AuditLogs.CountAsync(audit =>
                audit.Action == "ProjectTask.Created"));
        });
    }

    [Fact]
    public async Task SelfServiceRoutes_EnforceOwnershipReadStateAndReadOnlyCreation()
    {
        var secondUser = await fixture.SeedActiveUserAsync();
        var seeded = await fixture.WithDbContextAsync(async dbContext =>
        {
            var own = Notification.Create(
                fixture.ActiveUserId,
                "Own notification",
                "Visible to the active user.",
                NotificationTypes.ProjectTaskAssigned,
                700001);
            var otherFirst = Notification.Create(
                secondUser.UserId,
                "Other notification",
                "Visible only to its recipient.",
                NotificationTypes.ProjectMemberAdded,
                700002);
            var otherSecond = Notification.Create(
                secondUser.UserId,
                "Other newest notification",
                "Newest notification for ordering.",
                NotificationTypes.InvoiceIssued,
                700003);
            dbContext.Notifications.AddRange(own, otherFirst, otherSecond);
            await dbContext.SaveChangesAsync();
            return (Own: own, OtherFirst: otherFirst, OtherSecond: otherSecond);
        });

        using var activeClient = await fixture.CreateAuthenticatedClientAsync();
        using var ownListResponse = await activeClient.GetAsync("/api/notifications");
        Assert.Equal(HttpStatusCode.OK, ownListResponse.StatusCode);
        var ownList = (await ownListResponse.Content.ReadFromJsonAsync<List<NotificationDto>>())!;
        Assert.All(ownList, notification => Assert.Equal(fixture.ActiveUserId, notification.UserId));
        Assert.Contains(ownList, notification => notification.Id == seeded.Own.Id);
        Assert.DoesNotContain(ownList, notification => notification.Id == seeded.OtherFirst.Id);

        using var crossUserGet = await activeClient.GetAsync(
            $"/api/notifications/{seeded.OtherFirst.Id}");
        using var crossUserPatch = await activeClient.PatchAsJsonAsync(
            $"/api/notifications/{seeded.OtherFirst.Id}/read",
            new { IsRead = true });
        Assert.Equal(HttpStatusCode.NotFound, crossUserGet.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, crossUserPatch.StatusCode);

        using var secondClient = fixture.CreateClient();
        var secondToken = await NamaaApiFixture.LoginAsync(
            secondClient,
            secondUser.Email,
            secondUser.Password);
        secondClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", secondToken.AccessToken);

        using var otherListResponse = await secondClient.GetAsync("/api/notifications");
        Assert.Equal(HttpStatusCode.OK, otherListResponse.StatusCode);
        var otherList = (await otherListResponse.Content.ReadFromJsonAsync<List<NotificationDto>>())!;
        Assert.Equal(
            new[] { seeded.OtherSecond.Id, seeded.OtherFirst.Id },
            otherList.Select(notification => notification.Id));

        using var markReadResponse = await secondClient.PatchAsJsonAsync(
            $"/api/notifications/{seeded.OtherFirst.Id}/read",
            new { IsRead = true });
        Assert.Equal(HttpStatusCode.OK, markReadResponse.StatusCode);
        var markedRead = (await markReadResponse.Content.ReadFromJsonAsync<NotificationDto>())!;
        Assert.True(markedRead.IsRead);
        Assert.NotNull(markedRead.ReadAt);

        using var unreadResponse = await secondClient.GetAsync("/api/notifications?isRead=false");
        var unread = (await unreadResponse.Content.ReadFromJsonAsync<List<NotificationDto>>())!;
        Assert.DoesNotContain(unread, notification => notification.Id == seeded.OtherFirst.Id);

        using var arbitraryCreate = await activeClient.PostAsJsonAsync(
            "/api/notifications",
            new { UserId = secondUser.UserId, Title = "Spoofed", Message = "Spoofed" });
        using var removedRoute = await activeClient.GetAsync(
            $"/api/users/{secondUser.UserId}/notifications");
        using var anonymous = fixture.CreateClient();
        using var anonymousResponse = await anonymous.GetAsync("/api/notifications");
        Assert.Equal(HttpStatusCode.MethodNotAllowed, arbitraryCreate.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, removedRoute.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);

        await fixture.WithDbContextAsync(async dbContext =>
        {
            Assert.True(await dbContext.Notifications.AnyAsync(notification =>
                notification.Id == seeded.OtherFirst.Id &&
                notification.UserId == secondUser.UserId &&
                notification.IsRead &&
                notification.ReadAt != null));
            Assert.True(await dbContext.AuditLogs.AnyAsync(audit =>
                audit.Action == AuditActions.For(AuditEntities.Notification, AuditActions.ReadStateChanged) &&
                audit.EntityId == seeded.OtherFirst.Id &&
                audit.UserId == secondUser.UserId));
        });
    }
}
