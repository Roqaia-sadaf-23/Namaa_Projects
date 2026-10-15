using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Extensions;
using Namaa.Application.ProjectStatusHistory.Queries.GetProjectStatusHistory;
using Namaa.Application.ProjectStatusHistory.Queries.GetProjectStatusHistoryById;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:long}/status-history")]
public sealed class ProjectStatusHistoryController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(long projectId, CancellationToken cancellationToken) =>
        (await sender.Send(new GetProjectStatusHistoryQuery(projectId), cancellationToken)).ToActionResult(this);

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long projectId, long id, CancellationToken cancellationToken) =>
        (await sender.Send(new GetProjectStatusHistoryByIdQuery(projectId, id), cancellationToken)).ToActionResult(this);
}

