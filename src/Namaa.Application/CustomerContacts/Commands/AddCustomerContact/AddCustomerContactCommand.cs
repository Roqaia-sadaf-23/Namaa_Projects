using MediatR; using Namaa.Application.CustomerContacts.Dtos; using Namaa.Domain.Common.Results;
namespace Namaa.Application.CustomerContacts.Commands.AddCustomerContact;
public sealed record AddCustomerContactCommand(long CustomerId, string FullName, string? Email, string? PhoneNumber, string? JobTitle, bool IsPrimary) : IRequest<Result<CustomerContactDto>>;

