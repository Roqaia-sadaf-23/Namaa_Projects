using MediatR;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStages.Commands.CreateProjectStage;

public sealed record CreateProjectStageCommand(
    long ProjectId,
    string Name,
    int StageOrder,
    string Status,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<Result<ProjectStageDto>>;
