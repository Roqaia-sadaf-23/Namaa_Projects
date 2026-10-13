using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectFiles.Commands.CreateProjectFile;

public sealed class CreateProjectFileCommandHandler(
    IProjectFileRepository projectFileRepository,
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<CreateProjectFileCommand, Result<ProjectFileDto>>
{
    public async Task<Result<ProjectFileDto>> Handle(
        CreateProjectFileCommand request,
        CancellationToken cancellationToken)
    {
        if (await projectFileRepository.ExistsByStorageKeyAsync(
                request.StorageKey,
                cancellationToken))
        {
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.DuplicateStorageKey);
        }

        if (await projectRepository.FindByIdAsync(
                request.ProjectId,
                cancellationToken) is null)
        {
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.ProjectNotFound);
        }

        if (!await userRepository.ExistsByIdAsync(
                request.UploadedByUserId,
                cancellationToken))
        {
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.UploadedByUserNotFound);
        }

        var file = ProjectFile.Create(
            request.ProjectId,
            request.UploadedByUserId,
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
