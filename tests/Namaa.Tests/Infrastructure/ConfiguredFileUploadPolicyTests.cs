using Microsoft.Extensions.Options;
using Namaa.Domain.Common.Errors;
using Namaa.Infrastructure.FileStorage;

namespace Namaa.Tests.Infrastructure;

public sealed class ConfiguredFileUploadPolicyTests
{
    [Fact]
    public void Validate_EmptyFileName_IsRejected()
    {
        var result = CreatePolicy().Validate("   ", "text/plain", 1);

        Assert.True(result.IsFailure);
        Assert.Equal(ProjectFileErrors.InvalidFile, result.Error);
    }

    [Fact]
    public void Validate_TraversalFileName_IsRejected()
    {
        var policy = CreatePolicy();
        var unsafeFileNames = new[]
        {
            "../../report.txt",
            "..\\..\\report.txt",
            "%2e%2e/%2e%2e/report.txt",
            "%252e%252e%252freport.txt",
            "C:\\temp\\report.txt",
            "\\\\server\\share\\report.txt",
            "/etc/passwd"
        };

        foreach (var fileName in unsafeFileNames)
        {
            var result = policy.Validate(fileName, "text/plain", 1);

            Assert.True(result.IsFailure);
            Assert.Equal(ProjectFileErrors.InvalidFile, result.Error);
        }
    }

    [Fact]
    public void Validate_DeclaredSizeOverLimit_IsRejected()
    {
        var result = CreatePolicy(maxFileSizeBytes: 100).Validate("report.txt", "text/plain", 101);

        Assert.True(result.IsFailure);
        Assert.Equal(ProjectFileErrors.FileTooLarge, result.Error);
    }

    [Fact]
    public void Validate_ValidMetadata_IsNormalizedAndAccepted()
    {
        var result = CreatePolicy().Validate("folder/report.txt", " text/plain ", 10);

        Assert.True(result.IsSuccess);
        Assert.Equal("report.txt", result.Value.FileName);
        Assert.Equal("text/plain", result.Value.ContentType);
    }

    [Fact]
    public void Validate_BlockedExtension_IsRejectedCaseInsensitively()
    {
        var result = CreatePolicy().Validate("payload.EXE", "application/octet-stream", 10);

        Assert.True(result.IsFailure);
        Assert.Equal(ProjectFileErrors.FileTypeBlocked, result.Error);
    }

    private static ConfiguredFileUploadPolicy CreatePolicy(long maxFileSizeBytes = 1024) =>
        new(Options.Create(new FileStorageOptions
        {
            MaxFileSizeBytes = maxFileSizeBytes,
            BlockedExtensions = [".exe"],
            BlockedContentTypes = ["application/x-msdownload"]
        }));
}
