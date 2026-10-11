using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    ICustomerRepository customerRepository,
    IQuotationRepository quotationRepository,
    IUserRepository userRepository,
    IApplicationDbContext dbContext)
    : IRequestHandler<CreateProjectCommand, Result<ProjectDto>>
{
    public async Task<Result<ProjectDto>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        if (await projectRepository.ExistsByCodeAsync(
                request.ProjectCode,
                cancellationToken))
        {
            return Result.Failure<ProjectDto>(ProjectErrors.DuplicateProjectCode);
        }

        if (await customerRepository.FindByIdAsync(
                request.CustomerId,
                cancellationToken) is null)
        {
            return Result.Failure<ProjectDto>(ProjectErrors.CustomerNotFound);
        }

        if (!await userRepository.ExistsByIdAsync(
                request.CreatedByUserId,
                cancellationToken))
        {
            return Result.Failure<ProjectDto>(ProjectErrors.CreatedByUserNotFound);
        }

        if (request.SourceQuotationId.HasValue)
        {
            if (!await quotationRepository.ExistsByIdAsync(
                    request.SourceQuotationId.Value,
                    cancellationToken))
            {
                return Result.Failure<ProjectDto>(ProjectErrors.SourceQuotationNotFound);
            }

            if (await projectRepository.ExistsBySourceQuotationIdAsync(
                    request.SourceQuotationId.Value,
                    cancellationToken))
            {
                return Result.Failure<ProjectDto>(ProjectErrors.SourceQuotationAlreadyAssigned);
            }
        }

        if (request.ProjectManagerId.HasValue &&
            !await userRepository.ExistsByIdAsync(
                request.ProjectManagerId.Value,
                cancellationToken))
        {
            return Result.Failure<ProjectDto>(ProjectErrors.ProjectManagerNotFound);
        }

        var project = Project.Create(
            request.CustomerId,
            request.ProjectCode,
            request.Name,
            request.BudgetAmount,
            request.CreatedByUserId,
            request.SourceQuotationId,
            request.ProjectManagerId,
            request.Description,
            request.Location,
            request.StartDate,
            request.EndDate);

        await projectRepository.AddAsync(project, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectDto.FromEntity(project));
    }
}
