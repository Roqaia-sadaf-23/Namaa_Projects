namespace Namaa.Api.Contracts.ProjectStages;

public sealed record CreateProjectStageRequest(
    string Name,
    int StageOrder,
    string Status,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate);
