using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Commands.ChangeProjectStatus;

public sealed class ChangeProjectStatusCommandHandler(IProjectRepository projectRepository, IUserRepository userRepository, IApplicationDbContext dbContext) : IRequestHandler<ChangeProjectStatusCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(ChangeProjectStatusCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindForUpdateAsync(request.ProjectId, cancellationToken);
        if (project is null) return Result.Failure<ProjectDto>(ProjectErrors.NotFound);
        if (!await userRepository.ExistsByIdAsync(request.ChangedByUserId, cancellationToken)) return Result.Failure<ProjectDto>(ProjectStatusHistoryErrors.UserNotFound);
        project.ChangeStatus(request.Status, request.ChangedByUserId, request.Reason);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ProjectDto.FromEntity(project));
    }
}

