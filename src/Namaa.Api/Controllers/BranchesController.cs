using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.Branches;
using Namaa.Api.Extensions;
using Namaa.Application.Branches.Commands.CreateBranch;
using Namaa.Application.Branches.Commands.UpdateBranch;
using Namaa.Application.Branches.Queries.GetBranchById;
using Namaa.Application.Branches.Queries.GetBranches;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/branches")]
public sealed class BranchesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBranchRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateBranchCommand(request.BranchCode, request.Name, request.Address, request.City), cancellationToken);
        return result.IsFailure ? result.ToActionResult(this) : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken) =>
        (await sender.Send(new GetBranchByIdQuery(id), cancellationToken)).ToActionResult(this);

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        (await sender.Send(new GetBranchesQuery(), cancellationToken)).ToActionResult(this);

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, UpdateBranchRequest request, CancellationToken cancellationToken) =>
        (await sender.Send(new UpdateBranchCommand(id, request.Name, request.Address, request.City, request.RowVersion), cancellationToken)).ToActionResult(this);
}

