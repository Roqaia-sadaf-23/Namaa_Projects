using Namaa.Domain.Entities;

namespace Namaa.Application.ProjectStages.Dtos;

public sealed record ProjectStageDto(
    long Id,
    long ProjectId,
    string Name,
    string? Description,
    int StageOrder,
    DateTime? StartDate,
    DateTime? EndDate,
    string Status,
    decimal ProgressPercentage,
    DateTime CreatedAt)
{
    internal static ProjectStageDto FromEntity(ProjectStage stage) =>
        new(
            stage.Id,
            stage.ProjectId,
            stage.Name,
            stage.Description,
            stage.StageOrder,
            stage.StartDate,
            stage.EndDate,
            stage.Status,
            stage.ProgressPercentage,
            stage.CreatedAt);
}
