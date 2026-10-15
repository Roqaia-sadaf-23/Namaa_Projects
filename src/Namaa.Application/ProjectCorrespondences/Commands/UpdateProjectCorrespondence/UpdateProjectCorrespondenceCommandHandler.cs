using MediatR;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.ProjectCorrespondences.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectCorrespondences.Commands.UpdateProjectCorrespondence;

public sealed class UpdateProjectCorrespondenceCommandHandler(IProjectCorrespondenceRepository repository, IApplicationDbContext dbContext) : IRequestHandler<UpdateProjectCorrespondenceCommand, Result<ProjectCorrespondenceDto>>
{
    public async Task<Result<ProjectCorrespondenceDto>> Handle(UpdateProjectCorrespondenceCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.FindForUpdateAsync(request.ProjectId, request.Id, cancellationToken);
        if (entity is null)
            return Result.Failure<ProjectCorrespondenceDto>(ProjectCorrespondenceErrors.NotFound);
        entity.Update(request.CorrespondenceType, request.Subject, request.CorrespondenceDate, request.ReferenceNumber, request.Notes);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ProjectCorrespondenceDto.FromEntity(entity));
    }
}

