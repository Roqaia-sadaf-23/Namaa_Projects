using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.ProjectMembers;
using Namaa.Api.Extensions;
using Namaa.Application.ProjectMembers.Commands.AddProjectMember;
using Namaa.Application.ProjectMembers.Commands.UpdateProjectMember;
using Namaa.Application.ProjectMembers.Dtos;
using Namaa.Application.ProjectMembers.Queries.GetProjectMemberById;
using Namaa.Application.ProjectMembers.Queries.GetProjectMembers;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:long}/members")]
public sealed class ProjectMembersController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ProjectMemberDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(
        long projectId,
        AddProjectMemberRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddProjectMemberCommand(
            projectId,
            request.UserId,
            request.JoinedAt,
            request.ProjectRole);

        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(
                nameof(GetById),
                new { projectId, id = result.Value.Id },
                result.Value);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ProjectMemberDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(
        long projectId,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectMembersQuery(projectId),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType<ProjectMemberDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        long projectId,
        long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProjectMemberByIdQuery(projectId, id),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType<ProjectMemberDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long projectId,
        long id,
        UpdateProjectMemberRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateProjectMemberCommand(
                projectId,
                id,
                request.ProjectRole,
                request.LeftAt),
            cancellationToken);

        return result.ToActionResult(this);
    }
}
