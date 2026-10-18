using System.Buffers;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Namaa.Application.Abstractions.FileStorage;
using Namaa.Domain.Common.Errors;

namespace Namaa.Infrastructure.FileStorage;

public sealed partial class LocalFileStorage : IFileStorage
{
    private const int BufferSize = 81920;
    private readonly string _root;
    private readonly long _maxFileSizeBytes;
    private readonly ILogger<LocalFileStorage> _logger;

    private static readonly Action<ILogger, string, Exception?> LogStagingCleanupFailure =
        LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(1, "FileStorageStagingCleanupFailure"),
            "File-storage staging cleanup failed; failure type {FailureType}");

    private static readonly Action<ILogger, string, Exception?> LogStorageKeyRejected =
        LoggerMessage.Define<string>(
            LogLevel.Warning,
            new EventId(2, "FileStorageKeyRejected"),
            "Rejected an invalid file-storage key; reason {Reason}");

    public LocalFileStorage(
        IOptions<FileStorageOptions> options,
        ILogger<LocalFileStorage> logger)
    {
        _logger = logger;
        var configuredRoot = options.Value.LocalRoot;
        var root = string.IsNullOrWhiteSpace(configuredRoot)
            ? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Namaa",
                "ProjectFiles")
            : configuredRoot;

        if (!Path.IsPathRooted(root))
            throw new InvalidOperationException("FileStorage:LocalRoot must be an absolute path when configured.");

        _root = Path.GetFullPath(root);
        _maxFileSizeBytes = options.Value.MaxFileSizeBytes;
        Directory.CreateDirectory(_root);
        Directory.CreateDirectory(Path.Combine(_root, ".staging"));
    }

    public async Task<StoredFileResult> SaveAsync(
        Stream content,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var identifier = Guid.NewGuid().ToString("N");
        var storageKey = $"objects/{identifier[..2]}/{identifier}";
        var destination = ResolveStoragePath(storageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

        var stagingPath = Path.Combine(_root, ".staging", $"{Guid.NewGuid():N}.tmp");
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        long totalBytes = 0;

        try
        {
            await using (var output = new FileStream(
                             stagingPath,
                             FileMode.CreateNew,
                             FileAccess.Write,
                             FileShare.None,
                             BufferSize,
                             FileOptions.Asynchronous | FileOptions.SequentialScan))
            {
                while (true)
                {
                    var bytesRead = await content.ReadAsync(
                        buffer.AsMemory(0, BufferSize),
                        cancellationToken);
                    if (bytesRead == 0)
                        break;

                    totalBytes += bytesRead;
                    if (totalBytes > _maxFileSizeBytes)
                        throw new FileStorageValidationException(ProjectFileErrors.FileTooLarge);

                    await output.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                }

                await output.FlushAsync(cancellationToken);
            }

            if (totalBytes == 0)
                throw new FileStorageValidationException(ProjectFileErrors.InvalidFile);

            File.Move(stagingPath, destination, false);
            return new StoredFileResult(storageKey, totalBytes, contentType);
        }
        catch
        {
            TryDeleteStagingFile(stagingPath);
            throw;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public Task<Stream?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = ResolveStoragePath(storageKey);
        Stream? stream = File.Exists(path)
            ? new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan)
            : null;
        return Task.FromResult(stream);
    }

    public Task<bool> ExistsAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(File.Exists(ResolveStoragePath(storageKey)));
    }

    public Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var path = ResolveStoragePath(storageKey);
        if (File.Exists(path))
            File.Delete(path);
        return Task.CompletedTask;
    }

    private string ResolveStoragePath(string storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            LogStorageKeyRejected(_logger, "Empty", null);
            throw new ArgumentException("The storage key is required.", nameof(storageKey));
        }

        if (!StorageKeyRegex().IsMatch(storageKey))
        {
            LogStorageKeyRejected(_logger, "InvalidFormat", null);
            throw new ArgumentException("The storage key format is invalid.", nameof(storageKey));
        }

        var relativePath = storageKey.Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.GetFullPath(Path.Combine(_root, relativePath));
        var requiredPrefix = _root.EndsWith(Path.DirectorySeparatorChar)
            ? _root
            : _root + Path.DirectorySeparatorChar;
        var comparison = OperatingSystem.IsWindows()
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;
        if (!fullPath.StartsWith(requiredPrefix, comparison))
        {
            LogStorageKeyRejected(_logger, "OutsideRoot", null);
            throw new ArgumentException("The storage key resolves outside the storage root.", nameof(storageKey));
        }

        return fullPath;
    }

    private void TryDeleteStagingFile(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch (Exception exception)
        {
            LogStagingCleanupFailure(_logger, exception.GetType().Name, null);
        }
    }

    [GeneratedRegex("^objects/[0-9a-f]{2}/[0-9a-f]{32}$", RegexOptions.CultureInvariant)]
    private static partial Regex StorageKeyRegex();
}
