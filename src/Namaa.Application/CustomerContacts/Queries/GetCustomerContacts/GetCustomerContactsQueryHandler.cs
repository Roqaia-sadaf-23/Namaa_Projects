using MediatR;using Namaa.Application.Abstractions.Persistence;using Namaa.Application.CustomerContacts.Dtos;using Namaa.Domain.Common.Results;
namespace Namaa.Application.CustomerContacts.Queries.GetCustomerContacts;
public sealed class GetCustomerContactsQueryHandler(ICustomerRepository repository):IRequestHandler<GetCustomerContactsQuery,Result<IReadOnlyList<CustomerContactDto>>>{public async Task<Result<IReadOnlyList<CustomerContactDto>>> Handle(GetCustomerContactsQuery request,CancellationToken cancellationToken){var customer=await repository.FindByIdWithContactsAsync(request.CustomerId,cancellationToken);return Result.Success<IReadOnlyList<CustomerContactDto>>(customer?.Contacts.OrderBy(x=>x.Id).Select(CustomerContactDto.FromEntity).ToList()??[]);}}

