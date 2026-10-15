using MediatR;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Application.Common.Concurrency;
using Namaa.Domain.Common.Results;
using Namaa.Domain.Enums;

namespace Namaa.Application.ProjectTasks.Commands.UpdateProjectTask;

public sealed record UpdateProjectTaskCommand(
    long ProjectId,
    long Id,
    string Title,
    TaskPriority Priority,
    string? Description,
    DateTime? StartDate,
    DateTime? DueDate,
    long? AssignedToUserId,
    long? StageId,
    string? TaskType,
    string RowVersion) : IRequest<Result<ProjectTaskDto>>, IConcurrencyCommand;
