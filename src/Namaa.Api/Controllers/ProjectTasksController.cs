using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.ProjectTasks;
using Namaa.Api.Extensions;
using Namaa.Application.ProjectTasks.Commands.CreateProjectTask;
using Namaa.Application.ProjectTasks.Commands.UpdateProjectTask;
using Namaa.Application.ProjectTasks.Dtos;
using Namaa.Application.ProjectTasks.Queries.GetProjectTaskById;
using Namaa.Application.ProjectTasks.Queries.GetProjectTasks;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:long}/tasks")]
public sealed class ProjectTasksController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ProjectTaskDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        long projectId,
        CreateProjectTaskRequest request,
        CancellationToken cancellationToken)
    {
        // TODO: Source CreatedByUserId from authenticated user claims when authentication is implemented.
        var command = new CreateProjectTaskCommand(
            projectId,
            request.Title,
            request.CreatedByUserId,
            request.Priority,
            request.Description,
            request.StartDate,
            request.DueDate,
            request.AssignedToUserId,
            request.ParentTaskId,
            request.StageId,
            request.TaskType);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(
                nameof(GetById),
                new { projectId, id = result.Value.Id },
                result.Value);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProjectTaskDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(
        long projectId,
        [FromQuery] long? stageId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectTasksQuery(projectId, stageId),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType<ProjectTaskDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        long projectId,
        long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectTaskByIdQuery(projectId, id),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType<ProjectTaskDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long projectId,
        long id,
        UpdateProjectTaskRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectTaskCommand(
            projectId,
            id,
            request.Title,
            request.Priority,
            request.Description,
            request.StartDate,
            request.DueDate,
            request.AssignedToUserId,
            request.StageId,
            request.TaskType);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult(this);
    }
}
