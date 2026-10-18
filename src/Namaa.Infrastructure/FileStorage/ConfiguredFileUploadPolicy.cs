using Namaa.Application.Abstractions.FileStorage;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Microsoft.Extensions.Options;

namespace Namaa.Infrastructure.FileStorage;

public sealed class ConfiguredFileUploadPolicy : IFileUploadPolicy
{
    private readonly FileStorageOptions _options;
    private readonly HashSet<string> _blockedExtensions;
    private readonly HashSet<string> _blockedContentTypes;

    public ConfiguredFileUploadPolicy(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
        _blockedExtensions = new HashSet<string>(
            _options.BlockedExtensions.Select(NormalizeExtension),
            StringComparer.OrdinalIgnoreCase);
        _blockedContentTypes = new HashSet<string>(
            _options.BlockedContentTypes.Select(value => value.Trim()),
            StringComparer.OrdinalIgnoreCase);
    }

    public Result<ValidatedFileUpload> Validate(
        string fileName,
        string? contentType,
        long declaredSizeBytes)
    {
        if (declaredSizeBytes <= 0 || string.IsNullOrWhiteSpace(fileName))
            return Result.Failure<ValidatedFileUpload>(ProjectFileErrors.InvalidFile);

        if (declaredSizeBytes > _options.MaxFileSizeBytes)
            return Result.Failure<ValidatedFileUpload>(ProjectFileErrors.FileTooLarge);

        if (fileName.Any(char.IsControl))
            return Result.Failure<ValidatedFileUpload>(ProjectFileErrors.InvalidFile);

        var segments = fileName.Replace('\\', '/').Split(
            '/',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var safeFileName = segments.LastOrDefault();
        if (string.IsNullOrWhiteSpace(safeFileName) ||
            safeFileName is "." or ".." ||
            safeFileName.Length > 260)
        {
            return Result.Failure<ValidatedFileUpload>(ProjectFileErrors.InvalidFile);
        }

        var normalizedContentType = string.IsNullOrWhiteSpace(contentType)
            ? null
            : contentType.Trim();
        if (normalizedContentType is { Length: > 100 } ||
            normalizedContentType?.Any(char.IsControl) == true)
        {
            return Result.Failure<ValidatedFileUpload>(ProjectFileErrors.InvalidFile);
        }

        var extension = NormalizeExtension(Path.GetExtension(safeFileName));
        if (_blockedExtensions.Contains(extension) ||
            (normalizedContentType is not null && _blockedContentTypes.Contains(normalizedContentType)))
        {
            return Result.Failure<ValidatedFileUpload>(ProjectFileErrors.FileTypeBlocked);
        }

        return Result.Success(new ValidatedFileUpload(safeFileName, normalizedContentType));
    }

    private static string NormalizeExtension(string extension) =>
        string.IsNullOrWhiteSpace(extension)
            ? string.Empty
            : extension.StartsWith('.') ? extension.Trim() : $".{extension.Trim()}";
}
