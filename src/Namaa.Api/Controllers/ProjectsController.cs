using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.Projects;
using Namaa.Api.Extensions;
using Namaa.Application.Projects.Commands.CreateProject;
using Namaa.Application.Projects.Commands.UpdateProject;
using Namaa.Application.Projects.Commands.ChangeProjectStatus;
using Namaa.Application.Projects.Dtos;
using Namaa.Application.Projects.Queries.GetProjectById;
using Namaa.Application.Projects.Queries.GetProjects;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/projects")]
public sealed class ProjectsController(ISender sender) : ControllerBase
{
    [HttpPost("{id:long}/status")]
    public async Task<IActionResult> ChangeStatus(
        long id,
        Namaa.Api.Contracts.Projects.ChangeProjectStatusRequest request,
        CancellationToken cancellationToken) =>
        (await sender.Send(
            new ChangeProjectStatusCommand(id, request.Status, request.Reason, request.RowVersion),
            cancellationToken)).ToActionResult(this);

    [HttpPost]
    [ProducesResponseType<ProjectDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProjectCommand(
            request.CustomerId,
            request.ProjectCode,
            request.Name,
            request.BudgetAmount,
            request.SourceQuotationId,
            request.ProjectManagerId,
            request.Description,
            request.Location,
            request.StartDate,
            request.EndDate);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(
                nameof(GetById),
                new { id = result.Value.Id },
                result.Value);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType<ProjectDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectByIdQuery(id),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProjectDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectsQuery(),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType<ProjectDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateProjectCommand(
                id,
                request.Name,
                request.BudgetAmount,
                request.Description,
                request.Location,
                request.StartDate,
                request.EndDate,
                request.ProjectManagerId,
                request.RowVersion),
            cancellationToken);

        return result.ToActionResult(this);
    }
}
