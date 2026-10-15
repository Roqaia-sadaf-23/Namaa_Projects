using MediatR;using Namaa.Application.CustomerContacts.Dtos;using Namaa.Domain.Common.Results;
namespace Namaa.Application.CustomerContacts.Queries.GetCustomerContactById;
public sealed record GetCustomerContactByIdQuery(long CustomerId,long Id):IRequest<Result<CustomerContactDto>>;

