using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.ProjectFiles;
using Namaa.Api.Extensions;
using Namaa.Application.ProjectFiles.Commands.CreateProjectFile;
using Namaa.Application.ProjectFiles.Commands.UpdateProjectFile;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Application.ProjectFiles.Queries.GetProjectFileById;
using Namaa.Application.ProjectFiles.Queries.GetProjectFiles;

namespace Namaa.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:long}/files")]
public sealed class ProjectFilesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ProjectFileDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        long projectId,
        CreateProjectFileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProjectFileCommand(
            projectId,
            request.FileName,
            request.StorageKey,
            request.FileSizeBytes,
            request.ContentType,
            request.Description,
            request.UploadedAt);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(
                nameof(GetById),
                new { projectId, id = result.Value.Id },
                result.Value);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProjectFileDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(
        long projectId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectFilesQuery(projectId),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType<ProjectFileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        long projectId,
        long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectFileByIdQuery(projectId, id),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType<ProjectFileDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long projectId,
        long id,
        UpdateProjectFileRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateProjectFileCommand(
                projectId,
                id,
                request.Description),
            cancellationToken);

        return result.ToActionResult(this);
    }
}
