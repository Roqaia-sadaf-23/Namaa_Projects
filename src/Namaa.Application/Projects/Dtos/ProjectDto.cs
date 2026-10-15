using Namaa.Domain.Entities;
using Namaa.Domain.Enums;
using Namaa.Application.Common.Concurrency;

namespace Namaa.Application.Projects.Dtos;

public sealed record ProjectDto(
    long Id,
    long CustomerId,
    long? SourceQuotationId,
    long? ProjectManagerId,
    string ProjectCode,
    string Name,
    string? Description,
    string? Location,
    DateTime? StartDate,
    DateTime? EndDate,
    DateTime? ActualCompletionDate,
    ProjectStatus Status,
    decimal ProgressPercentage,
    decimal BudgetAmount,
    long CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string RowVersion)
{
    internal static ProjectDto FromEntity(Project project) =>
        new(
            project.Id,
            project.CustomerId,
            project.SourceQuotationId,
            project.ProjectManagerId,
            project.ProjectCode,
            project.Name,
            project.Description,
            project.Location,
            project.StartDate,
            project.EndDate,
            project.ActualCompletionDate,
            project.Status,
            project.ProgressPercentage,
            project.BudgetAmount,
            project.CreatedByUserId,
            project.CreatedAt,
            project.UpdatedAt,
            ConcurrencyToken.Encode(project.RowVersion));
}
