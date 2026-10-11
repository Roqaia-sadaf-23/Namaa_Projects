using MediatR;
using Namaa.Application.Projects.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    long Id,
    string Name,
    decimal BudgetAmount,
    string? Description,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    long? ProjectManagerId) : IRequest<Result<ProjectDto>>;
