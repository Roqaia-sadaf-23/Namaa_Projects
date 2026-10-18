using MediatR;
using Microsoft.Extensions.Logging;
using Namaa.Application.Abstractions.FileStorage;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Queries.DownloadProjectFile;

public sealed class DownloadProjectFileQueryHandler(
    IProjectFileRepository projectFileRepository,
    IFileStorage fileStorage,
    ILogger<DownloadProjectFileQueryHandler> logger)
    : IRequestHandler<DownloadProjectFileQuery, Result<ProjectFileDownload>>
{
    private static readonly Action<ILogger, long, long, Exception?> LogMissingFile =
        LoggerMessage.Define<long, long>(
            LogLevel.Warning,
            new EventId(1, "ProjectFileContentMissing"),
            "Stored content is missing for project {ProjectId} file {ProjectFileId}");

    private static readonly Action<ILogger, long, long, string, Exception?> LogDownloadFailure =
        LoggerMessage.Define<long, long, string>(
            LogLevel.Error,
            new EventId(2, "ProjectFileDownloadFailure"),
            "Project file download failed for project {ProjectId} file {ProjectFileId}; failure type {FailureType}");

    public async Task<Result<ProjectFileDownload>> Handle(
        DownloadProjectFileQuery request,
        CancellationToken cancellationToken)
    {
        var file = await projectFileRepository.FindByIdAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);
        if (file is null)
            return Result.Failure<ProjectFileDownload>(ProjectFileErrors.NotFound);

        try
        {
            var content = await fileStorage.OpenReadAsync(file.StorageKey, cancellationToken);
            if (content is null)
            {
                LogMissingFile(logger, request.ProjectId, request.Id, null);
                return Result.Failure<ProjectFileDownload>(ProjectFileErrors.PhysicalFileNotFound);
            }

            return Result.Success(new ProjectFileDownload(
                content,
                file.FileName,
                string.IsNullOrWhiteSpace(file.ContentType)
                    ? "application/octet-stream"
                    : file.ContentType));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            LogDownloadFailure(
                logger,
                request.ProjectId,
                request.Id,
                exception.GetType().Name,
                null);
            return Result.Failure<ProjectFileDownload>(ProjectFileErrors.StorageUnavailable);
        }
    }
}
