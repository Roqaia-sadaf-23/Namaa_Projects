namespace Namaa.Api.Contracts.Projects;

public sealed record CreateProjectRequest(
    long CustomerId,
    string ProjectCode,
    string Name,
    decimal BudgetAmount,
    long? SourceQuotationId,
    long? ProjectManagerId,
    string? Description,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate);
