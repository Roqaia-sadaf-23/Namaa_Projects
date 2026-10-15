using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.Suppliers;
using Namaa.Api.Extensions;
using Namaa.Application.Suppliers.Commands.CreateSupplier;
using Namaa.Application.Suppliers.Commands.UpdateSupplier;
using Namaa.Application.Suppliers.Queries.GetSupplierById;
using Namaa.Application.Suppliers.Queries.GetSuppliers;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/suppliers")]
public sealed class SuppliersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateSupplierRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateSupplierCommand(request.SupplierCode, request.Name), cancellationToken);
        return result.IsFailure ? result.ToActionResult(this) : CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken) =>
        (await sender.Send(new GetSupplierByIdQuery(id), cancellationToken)).ToActionResult(this);

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        (await sender.Send(new GetSuppliersQuery(), cancellationToken)).ToActionResult(this);

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, UpdateSupplierRequest request, CancellationToken cancellationToken) =>
        (await sender.Send(new UpdateSupplierCommand(id, request.Name, request.PhoneNumber, request.Email, request.Address, request.City, request.TaxNumber, request.CommercialRegister, request.Notes), cancellationToken)).ToActionResult(this);
}

