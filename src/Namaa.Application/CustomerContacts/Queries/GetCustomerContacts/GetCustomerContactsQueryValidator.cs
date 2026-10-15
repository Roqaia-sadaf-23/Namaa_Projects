using FluentValidation;namespace Namaa.Application.CustomerContacts.Queries.GetCustomerContacts;
public sealed class GetCustomerContactsQueryValidator:AbstractValidator<GetCustomerContactsQuery>{public GetCustomerContactsQueryValidator()=>RuleFor(x=>x.CustomerId).GreaterThan(0);}

