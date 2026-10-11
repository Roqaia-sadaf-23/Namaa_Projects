using MediatR;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStages.Queries.GetProjectStageById;

public sealed record GetProjectStageByIdQuery(
    long ProjectId,
    long Id) : IRequest<Result<ProjectStageDto>>;
