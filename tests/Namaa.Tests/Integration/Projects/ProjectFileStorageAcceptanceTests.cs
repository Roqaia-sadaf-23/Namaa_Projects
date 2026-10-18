using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Namaa.Application.Abstractions.FileStorage;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Tests.Common;

namespace Namaa.Tests.Integration.Projects;

[Collection(AcceptanceTestCollection.Name)]
public sealed class ProjectFileStorageAcceptanceTests(NamaaApiFixture fixture)
{
    [Fact]
    public async Task Upload_AuthenticatedUserStoresFileMetadataAndAuditWithoutExposingStorageKey()
    {
        var project = await fixture.SeedProjectAsync();
        var content = Encoding.UTF8.GetBytes("project file acceptance content");
        var spoofedStorageKey = $"caller/{Guid.NewGuid():N}";
        using var client = await fixture.CreateAuthenticatedClientAsync();
        using var response = await UploadAsync(
            client,
            project.Id,
            content,
            "report.txt",
            "text/plain",
            spoofedStorageKey);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<ProjectFileDto>(body, JsonOptions)!;
        Assert.Equal(fixture.ActiveUserId, dto.UploadedByUserId);
        Assert.Equal(content.Length, dto.FileSizeBytes);
        Assert.DoesNotContain("storageKey", body, StringComparison.OrdinalIgnoreCase);
        Assert.False(body.Contains(spoofedStorageKey, StringComparison.Ordinal));

        var storageKey = await fixture.WithDbContextAsync(async db =>
        {
            var metadata = await db.ProjectFiles.AsNoTracking().SingleAsync(file => file.Id == dto.Id);
            var audit = await db.AuditLogs.AsNoTracking().SingleAsync(log =>
                log.EntityId == dto.Id && log.Action == "ProjectFile.MetadataCreated");
            Assert.Equal(fixture.ActiveUserId, metadata.UploadedByUserId);
            Assert.Equal(fixture.ActiveUserId, audit.UserId);
            Assert.NotEqual(spoofedStorageKey, metadata.StorageKey);
            Assert.StartsWith("objects/", metadata.StorageKey, StringComparison.Ordinal);
            return metadata.StorageKey;
        });

        await using var scope = fixture.Factory.Services.CreateAsyncScope();
        var storage = scope.ServiceProvider.GetRequiredService<IFileStorage>();
        Assert.True(await storage.ExistsAsync(storageKey));
    }

    [Fact]
    public async Task Upload_AnonymousUserReturns401WithoutMetadataOrAudit()
    {
        var project = await fixture.SeedProjectAsync();
        var auditCount = await fixture.WithDbContextAsync(db => db.AuditLogs.CountAsync());
        using var client = fixture.CreateClient();
        using var response = await UploadAsync(
            client,
            project.Id,
            Encoding.UTF8.GetBytes("anonymous upload"),
            "anonymous.txt",
            "text/plain");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        await fixture.WithDbContextAsync(async db =>
        {
            Assert.False(await db.ProjectFiles.AnyAsync(file => file.ProjectId == project.Id));
            Assert.Equal(auditCount, await db.AuditLogs.CountAsync());
        });
    }

    [Fact]
    public async Task Download_StreamsOriginalContentWithSafeAttachmentName()
    {
        var project = await fixture.SeedProjectAsync();
        var expected = Encoding.UTF8.GetBytes("streamed download content");
        using var client = await fixture.CreateAuthenticatedClientAsync();
        using var upload = await UploadAsync(
            client,
            project.Id,
            expected,
            "download.txt",
            "text/plain");
        var metadata = (await upload.Content.ReadFromJsonAsync<ProjectFileDto>())!;

        using var response = await client.GetAsync(
            $"/api/projects/{project.Id}/files/{metadata.Id}/download");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("download.txt", response.Content.Headers.ContentDisposition?.FileNameStar);
        Assert.Equal(expected, await response.Content.ReadAsByteArrayAsync());
    }

    [Fact]
    public async Task Download_FileFromAnotherProjectReturns404()
    {
        var projectA = await fixture.SeedProjectAsync("File project A");
        var projectB = await fixture.SeedProjectAsync("File project B");
        using var client = await fixture.CreateAuthenticatedClientAsync();
        using var upload = await UploadAsync(
            client,
            projectB.Id,
            Encoding.UTF8.GetBytes("private to project B"),
            "private.txt",
            "text/plain");
        var metadata = (await upload.Content.ReadFromJsonAsync<ProjectFileDto>())!;

        using var response = await client.GetAsync(
            $"/api/projects/{projectA.Id}/files/{metadata.Id}/download");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Download_WhenPhysicalFileIsMissingReturns404WithoutLeakingPath()
    {
        var project = await fixture.SeedProjectAsync();
        using var client = await fixture.CreateAuthenticatedClientAsync();
        using var upload = await UploadAsync(
            client,
            project.Id,
            Encoding.UTF8.GetBytes("will be removed"),
            "missing.txt",
            "text/plain");
        var metadata = (await upload.Content.ReadFromJsonAsync<ProjectFileDto>())!;
        var storageKey = await fixture.WithDbContextAsync(db => db.ProjectFiles
            .Where(file => file.Id == metadata.Id)
            .Select(file => file.StorageKey)
            .SingleAsync());
        await using (var scope = fixture.Factory.Services.CreateAsyncScope())
        {
            await scope.ServiceProvider.GetRequiredService<IFileStorage>()
                .DeleteAsync(storageKey);
        }
        fixture.LogSink.Clear();

        using var response = await client.GetAsync(
            $"/api/projects/{project.Id}/files/{metadata.Id}/download");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.DoesNotContain(fixture.StorageRoot, body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(storageKey, body, StringComparison.Ordinal);
        Assert.Contains(fixture.LogSink.Events, logEvent =>
            logEvent.RenderMessage(CultureInfo.InvariantCulture).Contains(
                "Stored content is missing",
                StringComparison.Ordinal));
    }

    [Fact]
    public async Task Upload_OversizedFileReturns400WithoutPhysicalOrDatabaseArtifacts()
    {
        var project = await fixture.SeedProjectAsync();
        var auditCount = await fixture.WithDbContextAsync(db => db.AuditLogs.CountAsync());
        var existingFiles = Directory.Exists(fixture.StorageRoot)
            ? Directory.GetFiles(fixture.StorageRoot, "*", SearchOption.AllDirectories).Length
            : 0;
        using var client = await fixture.CreateAuthenticatedClientAsync();
        using var response = await UploadAsync(
            client,
            project.Id,
            new byte[1025],
            "too-large.bin",
            "application/octet-stream");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(existingFiles, Directory.GetFiles(
            fixture.StorageRoot,
            "*",
            SearchOption.AllDirectories).Length);
        await fixture.WithDbContextAsync(async db =>
        {
            Assert.False(await db.ProjectFiles.AnyAsync(file => file.ProjectId == project.Id));
            Assert.Equal(auditCount, await db.AuditLogs.CountAsync());
        });
    }

    [Fact]
    public async Task Upload_TraversalFilenameReturns400AndCannotEscapeStorageRoot()
    {
        var project = await fixture.SeedProjectAsync();
        var parent = Directory.GetParent(fixture.StorageRoot)!.FullName;
        var escapedPath = Path.Combine(parent, "escape.txt");
        var auditCount = await fixture.WithDbContextAsync(db => db.AuditLogs.CountAsync());
        using var client = await fixture.CreateAuthenticatedClientAsync();
        using var response = await UploadAsync(
            client,
            project.Id,
            Encoding.UTF8.GetBytes("safe despite filename"),
            "../../escape.txt",
            "text/plain");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(File.Exists(escapedPath));
        await fixture.WithDbContextAsync(async db =>
        {
            Assert.False(await db.ProjectFiles.AnyAsync(file => file.ProjectId == project.Id));
            Assert.Equal(auditCount, await db.AuditLogs.CountAsync());
        });
    }

    [Fact]
    public async Task Upload_BlockedExecutableTypeReturns400WithoutSuccessAudit()
    {
        var project = await fixture.SeedProjectAsync();
        var auditCount = await fixture.WithDbContextAsync(db => db.AuditLogs.CountAsync());
        using var client = await fixture.CreateAuthenticatedClientAsync();
        using var response = await UploadAsync(
            client,
            project.Id,
            Encoding.UTF8.GetBytes("not executable content"),
            "payload.exe",
            "application/x-msdownload");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        await fixture.WithDbContextAsync(async db =>
        {
            Assert.False(await db.ProjectFiles.AnyAsync(file => file.ProjectId == project.Id));
            Assert.Equal(auditCount, await db.AuditLogs.CountAsync());
        });
    }

    private static async Task<HttpResponseMessage> UploadAsync(
        HttpClient client,
        long projectId,
        byte[] content,
        string fileName,
        string contentType,
        string? spoofedStorageKey = null)
    {
        using var form = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(fileContent, "File", fileName);
        form.Add(new StringContent("Acceptance description"), "Description");
        form.Add(new StringContent(long.MaxValue.ToString(CultureInfo.InvariantCulture)), "UploadedByUserId");
        if (spoofedStorageKey is not null)
            form.Add(new StringContent(spoofedStorageKey), "StorageKey");

        return await client.PostAsync($"/api/projects/{projectId}/files/upload", form);
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
}
