using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.Roles;
using Namaa.Api.Extensions;
using Namaa.Application.Roles.Commands.CreateRole;
using Namaa.Application.Roles.Commands.UpdateRole;
using Namaa.Application.Roles.Queries.GetRoleById;
using Namaa.Application.Roles.Queries.GetRoles;

namespace Namaa.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/roles")]
public sealed class RolesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateRoleCommand(request.Name, request.Description), cancellationToken);
        return result.IsFailure ? result.ToActionResult(this) : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(byte id, CancellationToken cancellationToken) =>
        (await sender.Send(new GetRoleByIdQuery(id), cancellationToken)).ToActionResult(this);

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        (await sender.Send(new GetRolesQuery(), cancellationToken)).ToActionResult(this);

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(byte id, UpdateRoleRequest request, CancellationToken cancellationToken) =>
        (await sender.Send(new UpdateRoleCommand(id, request.Name, request.Description), cancellationToken)).ToActionResult(this);
}

