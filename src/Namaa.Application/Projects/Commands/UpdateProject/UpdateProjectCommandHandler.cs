using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(
    IProjectRepository projectRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<UpdateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindForUpdateAsync(
            request.Id,
            cancellationToken);

        if (project is null)
        {
            return Result.Failure<ProjectDto>(ProjectErrors.NotFound);
        }

        project.UpdateDetails(
            request.Name,
            request.BudgetAmount,
            request.Description,
            request.Location,
            request.StartDate,
            request.EndDate,
            request.ProjectManagerId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectDto.FromEntity(project));
    }
}
