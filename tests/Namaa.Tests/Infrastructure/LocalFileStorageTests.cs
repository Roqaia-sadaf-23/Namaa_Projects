using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Namaa.Application.Abstractions.FileStorage;
using Namaa.Domain.Common.Errors;
using Namaa.Infrastructure.FileStorage;

namespace Namaa.Tests.Infrastructure;

public sealed class LocalFileStorageTests : IDisposable
{
    private readonly string _storageRoot = Path.Combine(
        Path.GetTempPath(),
        "NamaaLocalFileStorageTests",
        Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SaveAsync_ActualStreamExceedsLimit_IsRejectedAndLeavesNoFile()
    {
        var storage = CreateStorage(maxFileSizeBytes: 8);
        await using var content = new MemoryStream(new byte[9], writable: false);

        var exception = await Assert.ThrowsAsync<FileStorageValidationException>(() =>
            storage.SaveAsync(content, "report.bin", "application/octet-stream"));

        Assert.Equal(ProjectFileErrors.FileTooLarge, exception.Error);
        Assert.Empty(Directory.GetFiles(_storageRoot, "*", SearchOption.AllDirectories));
    }

    [Fact]
    public async Task SaveAsync_ValidStream_CreatesOneOpaquePhysicalFile()
    {
        var storage = CreateStorage();
        byte[] expected = [1, 2, 3, 4];
        await using var content = new MemoryStream(expected, writable: false);

        var stored = await storage.SaveAsync(content, "report.txt", "text/plain");

        Assert.StartsWith("objects/", stored.StorageKey, StringComparison.Ordinal);
        Assert.Equal(expected.LongLength, stored.FileSizeBytes);
        Assert.Equal("text/plain", stored.ContentType);
        var physicalFiles = Directory.GetFiles(_storageRoot, "*", SearchOption.AllDirectories);
        Assert.Single(physicalFiles);
        Assert.DoesNotContain("report.txt", physicalFiles[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExistsAsync_SavedFile_ReturnsTrue()
    {
        var storage = CreateStorage();
        await using var content = new MemoryStream([1], writable: false);
        var stored = await storage.SaveAsync(content, "report.txt", "text/plain");

        var exists = await storage.ExistsAsync(stored.StorageKey);

        Assert.True(exists);
    }

    [Fact]
    public async Task OpenReadAsync_SavedFile_ReturnsOriginalBytes()
    {
        var storage = CreateStorage();
        byte[] expected = [11, 22, 33, 44, 55];
        await using var content = new MemoryStream(expected, writable: false);
        var stored = await storage.SaveAsync(content, "report.txt", "text/plain");

        await using var opened = await storage.OpenReadAsync(stored.StorageKey);

        Assert.NotNull(opened);
        var actual = new byte[expected.Length];
        await opened.ReadExactlyAsync(actual);
        Assert.Equal(-1, opened.ReadByte());
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DeleteAsync_SavedFile_RemovesPhysicalFile()
    {
        var storage = CreateStorage();
        await using var content = new MemoryStream([1, 2, 3], writable: false);
        var stored = await storage.SaveAsync(content, "report.txt", "text/plain");

        await storage.DeleteAsync(stored.StorageKey);

        Assert.False(await storage.ExistsAsync(stored.StorageKey));
        Assert.Empty(Directory.GetFiles(_storageRoot, "*", SearchOption.AllDirectories));
    }

    [Fact]
    public async Task StorageOperations_TraversalOrAbsoluteKey_IsRejected()
    {
        var storage = CreateStorage();
        var unsafeStorageKeys = new[]
        {
            "../escape",
            "..\\escape",
            "%2e%2e/escape",
            "C:\\temp\\escape",
            "\\\\server\\share\\escape"
        };

        foreach (var storageKey in unsafeStorageKeys)
        {
            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await storage.ExistsAsync(storageKey));
        }
    }

    public void Dispose()
    {
        if (Directory.Exists(_storageRoot))
            Directory.Delete(_storageRoot, recursive: true);
    }

    private LocalFileStorage CreateStorage(long maxFileSizeBytes = 1024) =>
        new(
            Options.Create(new FileStorageOptions
            {
                Provider = "Local",
                LocalRoot = _storageRoot,
                MaxFileSizeBytes = maxFileSizeBytes
            }),
            NullLogger<LocalFileStorage>.Instance);
}
