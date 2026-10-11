using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namaa.Api.Contracts.Customers;
using Namaa.Api.Extensions;
using Namaa.Application.Customers.Commands.CreateCustomer;
using Namaa.Application.Customers.Commands.UpdateCustomer;
using Namaa.Application.Customers.Dtos;
using Namaa.Application.Customers.Queries.GetCustomerById;
using Namaa.Application.Customers.Queries.GetCustomers;

namespace Namaa.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        CreateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateCustomerCommand(
                request.CustomerCode,
                request.Name,
                request.CustomerType),
            cancellationToken);

        return result.IsFailure
            ? result.ToActionResult(this)
            : CreatedAtAction(
                nameof(GetById),
                new { id = result.Value.Id },
                result.Value);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        long id,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCustomerByIdQuery(id),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CustomerDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetCustomersQuery(),
            cancellationToken);

        return result.ToActionResult(this);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType<CustomerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long id,
        UpdateCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateCustomerCommand(
                id,
                request.Name,
                request.CustomerType,
                request.Email,
                request.PhoneNumber,
                request.Address,
                request.City,
                request.TaxNumber,
                request.CommercialRegister,
                request.Source,
                request.Notes),
            cancellationToken);

        return result.ToActionResult(this);
    }
}
