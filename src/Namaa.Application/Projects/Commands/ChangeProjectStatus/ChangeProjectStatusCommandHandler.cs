using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Errors;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Commands.ChangeProjectStatus;

public sealed class ChangeProjectStatusCommandHandler(IProjectRepository projectRepository, IUserRepository userRepository, ICurrentUser currentUser, IApplicationDbContext dbContext) : IRequestHandler<ChangeProjectStatusCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(ChangeProjectStatusCommand request, CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not long actorUserId) return Result.Failure<ProjectDto>(ApplicationErrors.Unauthenticated);
        var project = await projectRepository.FindForUpdateAsync(request.ProjectId, cancellationToken);
        if (project is null) return Result.Failure<ProjectDto>(ProjectErrors.NotFound);
        if (!await userRepository.ExistsByIdAsync(actorUserId, cancellationToken)) return Result.Failure<ProjectDto>(ProjectStatusHistoryErrors.UserNotFound);
        project.ChangeStatus(request.Status, actorUserId, request.Reason);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ProjectDto.FromEntity(project));
    }
}

