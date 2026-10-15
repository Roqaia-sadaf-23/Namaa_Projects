using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectCorrespondences.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectCorrespondences.Commands.CreateProjectCorrespondence;

public sealed class CreateProjectCorrespondenceCommandHandler(IProjectCorrespondenceRepository repository, IProjectRepository projectRepository, IUserRepository userRepository, IApplicationDbContext dbContext) : IRequestHandler<CreateProjectCorrespondenceCommand, Result<ProjectCorrespondenceDto>>
{
    public async Task<Result<ProjectCorrespondenceDto>> Handle(CreateProjectCorrespondenceCommand request, CancellationToken cancellationToken)
    {
        if (await projectRepository.FindByIdAsync(request.ProjectId, cancellationToken) is null)
            return Result.Failure<ProjectCorrespondenceDto>(ProjectCorrespondenceErrors.ProjectNotFound);
        if (!await userRepository.ExistsByIdAsync(request.CreatedByUserId, cancellationToken))
            return Result.Failure<ProjectCorrespondenceDto>(ProjectCorrespondenceErrors.UserNotFound);
        var entity = ProjectCorrespondence.Create(request.ProjectId, request.CorrespondenceType, request.Subject, request.CorrespondenceDate, request.CreatedByUserId, request.ReferenceNumber, request.Notes);
        await repository.AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ProjectCorrespondenceDto.FromEntity(entity));
    }
}

