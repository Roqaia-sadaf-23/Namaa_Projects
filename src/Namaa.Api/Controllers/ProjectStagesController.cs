using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.ProjectStages;
using Namaa.Api.Extensions;
using Namaa.Application.ProjectStages.Commands.CreateProjectStage;
using Namaa.Application.ProjectStages.Commands.UpdateProjectStage;
using Namaa.Application.ProjectStages.Dtos;
using Namaa.Application.ProjectStages.Queries.GetProjectStageById;
using Namaa.Application.ProjectStages.Queries.GetProjectStages;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:long}/stages")]
public sealed class ProjectStagesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ProjectStageDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        long projectId,
        CreateProjectStageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProjectStageCommand(
            projectId,
            request.Name,
            request.StageOrder,
            request.Status,
            request.Description,
            request.StartDate,
            request.EndDate);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(
                nameof(GetById),
                new { projectId, id = result.Value.Id },
                result.Value);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProjectStageDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(
        long projectId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectStagesQuery(projectId),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType<ProjectStageDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        long projectId,
        long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectStageByIdQuery(projectId, id),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType<ProjectStageDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long projectId,
        long id,
        UpdateProjectStageRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProjectStageCommand(
            projectId,
            id,
            request.Name,
            request.StageOrder,
            request.Status,
            request.ProgressPercentage,
            request.Description,
            request.StartDate,
            request.EndDate);

        var result = await sender.Send(command, cancellationToken);

        return result.ToActionResult(this);
    }
}
