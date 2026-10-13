using Namaa.Domain.Enums;

namespace Namaa.Api.Contracts.ProjectTasks;

public sealed record CreateProjectTaskRequest(
    string Title,
    long CreatedByUserId,
    TaskPriority Priority,
    string? Description,
    DateTime? StartDate,
    DateTime? DueDate,
    long? AssignedToUserId,
    long? ParentTaskId,
    long? StageId,
    string? TaskType);
