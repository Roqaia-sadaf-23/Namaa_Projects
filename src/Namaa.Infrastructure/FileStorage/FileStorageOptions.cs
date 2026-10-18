namespace Namaa.Infrastructure.FileStorage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorage";
    public const long DefaultMaxFileSizeBytes = 25 * 1024 * 1024;

    public string Provider { get; init; } = "Local";
    public string? LocalRoot { get; init; }
    public long MaxFileSizeBytes { get; init; } = DefaultMaxFileSizeBytes;
    public string[] BlockedExtensions { get; init; } = [];
    public string[] BlockedContentTypes { get; init; } = [];
}
