using MediatR;using Namaa.Application.CustomerContacts.Dtos;using Namaa.Domain.Common.Results;
namespace Namaa.Application.CustomerContacts.Queries.GetCustomerContacts;
public sealed record GetCustomerContactsQuery(long CustomerId):IRequest<Result<IReadOnlyList<CustomerContactDto>>>;

