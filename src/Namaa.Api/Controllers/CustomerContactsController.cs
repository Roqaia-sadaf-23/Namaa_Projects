using MediatR;using Microsoft.AspNetCore.Mvc;using Namaa.Api.Contracts.CustomerContacts;using Namaa.Api.Extensions;using Namaa.Application.CustomerContacts.Commands.AddCustomerContact;using Namaa.Application.CustomerContacts.Commands.UpdateCustomerContact;using Namaa.Application.CustomerContacts.Queries.GetCustomerContactById;using Namaa.Application.CustomerContacts.Queries.GetCustomerContacts;
namespace Namaa.Api.Controllers;
[ApiController][Route("api/customers/{customerId:long}/contacts")]
public sealed class CustomerContactsController(ISender sender):ControllerBase{
[HttpPost]public async Task<IActionResult>Create(long customerId,AddCustomerContactRequest request,CancellationToken cancellationToken){var x=await sender.Send(new AddCustomerContactCommand(customerId,request.FullName,request.Email,request.PhoneNumber,request.JobTitle,request.IsPrimary),cancellationToken);return x.IsFailure?x.ToActionResult(this):CreatedAtAction(nameof(GetById),new{customerId,id=x.Value.Id},x.Value);}
[HttpGet]public async Task<IActionResult>GetAll(long customerId,CancellationToken cancellationToken)=>(await sender.Send(new GetCustomerContactsQuery(customerId),cancellationToken)).ToActionResult(this);
[HttpGet("{id:long}")]public async Task<IActionResult>GetById(long customerId,long id,CancellationToken cancellationToken)=>(await sender.Send(new GetCustomerContactByIdQuery(customerId,id),cancellationToken)).ToActionResult(this);
[HttpPut("{id:long}")]public async Task<IActionResult>Update(long customerId,long id,UpdateCustomerContactRequest request,CancellationToken cancellationToken)=>(await sender.Send(new UpdateCustomerContactCommand(customerId,id,request.FullName,request.Email,request.PhoneNumber,request.JobTitle,request.IsPrimary),cancellationToken)).ToActionResult(this);}

