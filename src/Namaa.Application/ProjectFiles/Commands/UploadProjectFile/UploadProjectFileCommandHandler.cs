using MediatR;
using Microsoft.Extensions.Logging;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.FileStorage;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Errors;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectFiles.Commands.UploadProjectFile;

public sealed class UploadProjectFileCommandHandler(
    IProjectFileRepository projectFileRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IFileStorage fileStorage,
    IFileUploadPolicy uploadPolicy,
    ICurrentUser currentUser,
    IApplicationDbContext dbContext,
    ILogger<UploadProjectFileCommandHandler> logger)
    : IRequestHandler<UploadProjectFileCommand, Result<ProjectFileDto>>
{
    private static readonly Action<ILogger, long, string, Exception?> LogStorageFailure =
        LoggerMessage.Define<long, string>(
            LogLevel.Error,
            new EventId(1, "ProjectFileStorageFailure"),
            "Project file storage failed for project {ProjectId}; failure type {FailureType}");

    private static readonly Action<ILogger, long, string, Exception?> LogCleanupFailure =
        LoggerMessage.Define<long, string>(
            LogLevel.Error,
            new EventId(2, "ProjectFileCleanupFailure"),
            "Project file compensation failed for project {ProjectId}; failure type {FailureType}");

    public async Task<Result<ProjectFileDto>> Handle(
        UploadProjectFileCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not long actorUserId)
            return Result.Failure<ProjectFileDto>(ApplicationErrors.Unauthenticated);

        var validation = uploadPolicy.Validate(
            request.FileName,
            request.ContentType,
            request.FileSizeBytes);
        if (validation.IsFailure)
            return Result.Failure<ProjectFileDto>(validation.Error);

        if (await projectRepository.FindByIdAsync(request.ProjectId, cancellationToken) is null)
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.ProjectNotFound);

        if (!await userRepository.ExistsByIdAsync(actorUserId, cancellationToken))
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.UploadedByUserNotFound);

        StoredFileResult stored;
        try
        {
            stored = await fileStorage.SaveAsync(
                request.Content,
                validation.Value.FileName,
                validation.Value.ContentType,
                cancellationToken);
        }
        catch (FileStorageValidationException exception)
        {
            return Result.Failure<ProjectFileDto>(exception.Error);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            LogStorageFailure(logger, request.ProjectId, exception.GetType().Name, null);
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.StorageUnavailable);
        }

        try
        {
            var file = ProjectFile.Create(
                request.ProjectId,
                actorUserId,
                validation.Value.FileName,
                stored.StorageKey,
                stored.FileSizeBytes,
                stored.ContentType,
                request.Description);

            await projectFileRepository.AddAsync(file, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success(ProjectFileDto.FromEntity(file));
        }
        catch
        {
            await TryCompensateAsync(stored.StorageKey, request.ProjectId);
            throw;
        }
    }

    private async Task TryCompensateAsync(string storageKey, long projectId)
    {
        try
        {
            await fileStorage.DeleteAsync(storageKey, CancellationToken.None);
        }
        catch (Exception exception)
        {
            LogCleanupFailure(logger, projectId, exception.GetType().Name, null);
        }
    }
}
