using MediatR;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(
    long CustomerId,
    string ProjectCode,
    string Name,
    decimal BudgetAmount,
    long CreatedByUserId,
    long? SourceQuotationId,
    long? ProjectManagerId,
    string? Description,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<Result<ProjectDto>>;
