using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Errors;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectFiles.Commands.CreateProjectFile;

public sealed class CreateProjectFileCommandHandler(
    IProjectFileRepository projectFileRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    ICurrentUser currentUser,
    IApplicationDbContext dbContext)
    : IRequestHandler<CreateProjectFileCommand, Result<ProjectFileDto>>
{
    public async Task<Result<ProjectFileDto>> Handle(
        CreateProjectFileCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not long actorUserId)
            return Result.Failure<ProjectFileDto>(ApplicationErrors.Unauthenticated);

        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.ProjectNotFound);
        }

        if (!await userRepository.ExistsByIdAsync(
                actorUserId,
                cancellationToken))
        {
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.UploadedByUserNotFound);
        }

        if (await projectFileRepository.ExistsByStorageKeyAsync(
                request.StorageKey,
                cancellationToken))
        {
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.DuplicateStorageKey);
        }

        var file = ProjectFile.Create(
            request.ProjectId,
            actorUserId,
            request.FileName,
            request.StorageKey,
            request.FileSizeBytes,
            request.ContentType,
            request.Description,
            request.UploadedAt);

        await projectFileRepository.AddAsync(file, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectFileDto.FromEntity(file));
    }
}
