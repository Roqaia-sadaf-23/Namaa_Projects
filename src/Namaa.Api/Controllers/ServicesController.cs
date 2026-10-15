using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.Services;
using Namaa.Api.Extensions;
using Namaa.Application.Services.Commands.CreateService;
using Namaa.Application.Services.Commands.UpdateService;
using Namaa.Application.Services.Queries.GetServiceById;
using Namaa.Application.Services.Queries.GetServices;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/services")]
public sealed class ServicesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateServiceCommand(request.Name, request.DefaultPrice, request.Category, request.Description), cancellationToken);
        return result.IsFailure ? result.ToActionResult(this) : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken) =>
        (await sender.Send(new GetServiceByIdQuery(id), cancellationToken)).ToActionResult(this);

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        (await sender.Send(new GetServicesQuery(), cancellationToken)).ToActionResult(this);

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, UpdateServiceRequest request, CancellationToken cancellationToken) =>
        (await sender.Send(new UpdateServiceCommand(id, request.Name, request.DefaultPrice, request.Category, request.Description), cancellationToken)).ToActionResult(this);
}

