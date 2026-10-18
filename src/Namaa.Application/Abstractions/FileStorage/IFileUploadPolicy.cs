using Namaa.Domain.Common.Results;

namespace Namaa.Application.Abstractions.FileStorage;

public interface IFileUploadPolicy
{
    Result<ValidatedFileUpload> Validate(
        string fileName,
        string? contentType,
        long declaredSizeBytes);
}

public sealed record ValidatedFileUpload(
    string FileName,
    string? ContentType);
