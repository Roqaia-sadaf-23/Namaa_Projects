using FluentValidation; namespace Namaa.Application.CustomerContacts.Queries.GetCustomerContactById;
public sealed class GetCustomerContactByIdQueryValidator:AbstractValidator<GetCustomerContactByIdQuery>{public GetCustomerContactByIdQueryValidator(){RuleFor(x=>x.CustomerId).GreaterThan(0);RuleFor(x=>x.Id).GreaterThan(0);}}

