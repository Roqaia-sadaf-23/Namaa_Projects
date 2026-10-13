using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectFiles.Commands.UpdateProjectFile;

public sealed class UpdateProjectFileCommandHandler(
    IProjectFileRepository projectFileRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<UpdateProjectFileCommand, Result<ProjectFileDto>>
{
    public async Task<Result<ProjectFileDto>> Handle(
        UpdateProjectFileCommand request,
        CancellationToken cancellationToken)
    {
        var file = await projectFileRepository.FindForUpdateAsync(
            request.ProjectId,
            request.Id,
            cancellationToken);

        if (file is null)
        {
            return Result.Failure<ProjectFileDto>(ProjectFileErrors.NotFound);
        }

        file.UpdateDescription(request.Description);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectFileDto.FromEntity(file));
    }
}
