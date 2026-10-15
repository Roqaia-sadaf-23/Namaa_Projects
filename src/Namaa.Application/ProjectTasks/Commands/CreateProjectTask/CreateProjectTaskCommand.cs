using MediatR;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Enums;

namespace Namaa.Application.ProjectTasks.Commands.CreateProjectTask;

public sealed record CreateProjectTaskCommand(
    long ProjectId,
    string Title,
    TaskPriority Priority,
    string? Description,
    DateTime? StartDate,
    DateTime? DueDate,
    long? AssignedToUserId,
    long? ParentTaskId,
    long? StageId,
    string? TaskType) : IRequest<Result<ProjectTaskDto>>;
