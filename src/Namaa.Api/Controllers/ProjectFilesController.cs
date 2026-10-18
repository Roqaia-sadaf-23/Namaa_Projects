using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.ProjectFiles;
using Namaa.Api.Extensions;
using Namaa.Application.ProjectFiles.Commands.UploadProjectFile;
using Namaa.Application.ProjectFiles.Commands.UpdateProjectFile;
using Namaa.Application.ProjectFiles.Dtos;
using Namaa.Application.ProjectFiles.Queries.DownloadProjectFile;
using Namaa.Application.ProjectFiles.Queries.GetProjectFileById;
using Namaa.Application.ProjectFiles.Queries.GetProjectFiles;

namespace Namaa.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/projects/{projectId:long}/files")]
public sealed class ProjectFilesController(ISender sender) : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType<ProjectFileDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Upload(
        long projectId,
        [FromForm] UploadProjectFileRequest request,
        CancellationToken cancellationToken)
    {
        await using var content = request.File.OpenReadStream();
        var command = new UploadProjectFileCommand(
            projectId,
            content,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            request.Description);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(
                nameof(GetById),
                new { projectId, id = result.Value.Id },
                result.Value);
    }

    [HttpGet("{id:long}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Download(
        long projectId,
        long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DownloadProjectFileQuery(projectId, id),
            cancellationToken);

        return result.IsFailure
            ? result.ToActionResult(this)
            : File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
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
