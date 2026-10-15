using MediatR; using Namaa.Application.Abstractions.Persistence; using Namaa.Application.CustomerContacts.Dtos; using Namaa.Domain.Common.Errors; using Namaa.Domain.Common.Results;
namespace Namaa.Application.CustomerContacts.Commands.AddCustomerContact;
public sealed class AddCustomerContactCommandHandler(ICustomerRepository repository,IApplicationDbContext dbContext):IRequestHandler<AddCustomerContactCommand,Result<CustomerContactDto>>
{
 public async Task<Result<CustomerContactDto>> Handle(AddCustomerContactCommand request,CancellationToken cancellationToken){var customer=await repository.FindForUpdateWithContactsAsync(request.CustomerId,cancellationToken); if(customer is null)return Result.Failure<CustomerContactDto>(CustomerContactErrors.CustomerNotFound); if(request.IsPrimary&&customer.Contacts.Any(x=>x.IsPrimary))return Result.Failure<CustomerContactDto>(CustomerContactErrors.PrimaryAlreadyExists); var contact=customer.AddContact(request.FullName,request.Email,request.PhoneNumber,request.JobTitle,request.IsPrimary); await dbContext.SaveChangesAsync(cancellationToken); return Result.Success(CustomerContactDto.FromEntity(contact));}
}

