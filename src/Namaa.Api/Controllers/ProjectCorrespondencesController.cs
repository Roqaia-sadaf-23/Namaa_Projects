using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.ProjectCorrespondences;
using Namaa.Api.Extensions;
using Namaa.Application.ProjectCorrespondences.Commands.CreateProjectCorrespondence;
using Namaa.Application.ProjectCorrespondences.Commands.UpdateProjectCorrespondence;
using Namaa.Application.ProjectCorrespondences.Queries.GetProjectCorrespondenceById;
using Namaa.Application.ProjectCorrespondences.Queries.GetProjectCorrespondences;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:long}/correspondences")]
public sealed class ProjectCorrespondencesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(long projectId, CreateProjectCorrespondenceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateProjectCorrespondenceCommand(projectId, request.CorrespondenceType, request.Subject, request.CorrespondenceDate, request.CreatedByUserId, request.ReferenceNumber, request.Notes), cancellationToken);
        return result.IsFailure ? result.ToActionResult(this) : CreatedAtAction(nameof(GetById), new { projectId, id = result.Value.Id }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(long projectId, CancellationToken cancellationToken) =>
        (await sender.Send(new GetProjectCorrespondencesQuery(projectId), cancellationToken)).ToActionResult(this);

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long projectId, long id, CancellationToken cancellationToken) =>
        (await sender.Send(new GetProjectCorrespondenceByIdQuery(projectId, id), cancellationToken)).ToActionResult(this);

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long projectId, long id, UpdateProjectCorrespondenceRequest request, CancellationToken cancellationToken) =>
        (await sender.Send(new UpdateProjectCorrespondenceCommand(projectId, id, request.CorrespondenceType, request.Subject, request.CorrespondenceDate, request.ReferenceNumber, request.Notes), cancellationToken)).ToActionResult(this);
}

