namespace Namaa.Api.Contracts.ProjectStages;

public sealed record UpdateProjectStageRequest(
    string Name,
    int StageOrder,
    string Status,
    decimal ProgressPercentage,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate);
