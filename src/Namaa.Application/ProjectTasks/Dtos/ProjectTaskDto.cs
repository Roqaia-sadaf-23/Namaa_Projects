using Namaa.Domain.Entities;
using Namaa.Domain.Enums;

namespace Namaa.Application.ProjectTasks.Dtos;

public sealed record ProjectTaskDto(
    long Id,
    long ProjectId,
    long? ParentTaskId,
    long? AssignedToUserId,
    string Title,
    string? Description,
    ProjectTaskStatus Status,
    TaskPriority Priority,
    DateTime? StartDate,
    DateTime? DueDate,
    DateTime? CompletedAt,
    decimal ProgressPercentage,
    long CreatedByUserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    long? StageId,
    string? TaskType)
{
    internal static ProjectTaskDto FromEntity(ProjectTask task) =>
        new(
            task.Id,
            task.ProjectId,
            task.ParentTaskId,
            task.AssignedToUserId,
            task.Title,
            task.Description,
            task.Status,
            task.Priority,
            task.StartDate,
            task.DueDate,
            task.CompletedAt,
            task.ProgressPercentage,
            task.CreatedByUserId,
            task.CreatedAt,
            task.UpdatedAt,
            task.StageId,
            task.TaskType);
}
