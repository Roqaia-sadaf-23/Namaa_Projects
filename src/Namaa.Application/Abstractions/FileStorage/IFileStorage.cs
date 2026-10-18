namespace Namaa.Application.Abstractions.FileStorage;

public interface IFileStorage
{
    Task<StoredFileResult> SaveAsync(
        Stream content,
        string fileName,
        string? contentType,
        CancellationToken cancellationToken = default);

    Task<Stream?> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string storageKey,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken = default);
}

public sealed record StoredFileResult(
    string StorageKey,
    long FileSizeBytes,
    string? ContentType);
