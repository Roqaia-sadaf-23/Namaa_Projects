using MediatR;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.ProjectStages.Commands.UpdateProjectStage;

public sealed record UpdateProjectStageCommand(
    long ProjectId,
    long Id,
    string Name,
    int StageOrder,
    string Status,
    decimal ProgressPercentage,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<Result<ProjectStageDto>>;
