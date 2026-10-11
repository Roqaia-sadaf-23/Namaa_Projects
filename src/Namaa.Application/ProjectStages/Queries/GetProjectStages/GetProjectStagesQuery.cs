using MediatR;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStages.Queries.GetProjectStages;

public sealed record GetProjectStagesQuery(
    long ProjectId) : IRequest<Result<IReadOnlyList<ProjectStageDto>>>;
