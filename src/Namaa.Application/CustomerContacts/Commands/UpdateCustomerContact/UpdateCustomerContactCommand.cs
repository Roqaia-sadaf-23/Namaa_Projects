using MediatR; using Namaa.Application.CustomerContacts.Dtos; using Namaa.Domain.Common.Results;
namespace Namaa.Application.CustomerContacts.Commands.UpdateCustomerContact;
public sealed record UpdateCustomerContactCommand(long CustomerId,long Id,string FullName,string? Email,string? PhoneNumber,string? JobTitle,bool IsPrimary):IRequest<Result<CustomerContactDto>>;

