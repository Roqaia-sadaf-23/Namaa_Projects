using Namaa.Domain.Enums;

namespace Namaa.Api.Contracts.ProjectTasks;

public sealed record UpdateProjectTaskRequest(
    string Title,
    TaskPriority Priority,
    string? Description,
    DateTime? StartDate,
    DateTime? DueDate,
    long? AssignedToUserId,
    long? StageId,
    string? TaskType);
