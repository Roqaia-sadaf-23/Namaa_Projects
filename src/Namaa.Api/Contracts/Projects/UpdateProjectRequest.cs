namespace Namaa.Api.Contracts.Projects;

public sealed record UpdateProjectRequest(
    string Name,
    decimal BudgetAmount,
    string? Description,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    long? ProjectManagerId);
