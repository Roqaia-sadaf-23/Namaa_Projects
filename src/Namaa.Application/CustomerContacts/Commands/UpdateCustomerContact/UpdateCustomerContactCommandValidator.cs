using FluentValidation;
namespace Namaa.Application.CustomerContacts.Commands.UpdateCustomerContact;
public sealed class UpdateCustomerContactCommandValidator:AbstractValidator<UpdateCustomerContactCommand>{public UpdateCustomerContactCommandValidator(){RuleFor(x=>x.CustomerId).GreaterThan(0);RuleFor(x=>x.Id).GreaterThan(0);RuleFor(x=>x.FullName).NotEmpty().MaximumLength(200);RuleFor(x=>x.Email).MaximumLength(254).EmailAddress().When(x=>!string.IsNullOrWhiteSpace(x.Email));RuleFor(x=>x.PhoneNumber).MaximumLength(30);RuleFor(x=>x.JobTitle).MaximumLength(150);RuleFor(x=>x).Must(x=>!string.IsNullOrWhiteSpace(x.Email)||!string.IsNullOrWhiteSpace(x.PhoneNumber)).WithMessage("Either email or phone number is required.");}}

