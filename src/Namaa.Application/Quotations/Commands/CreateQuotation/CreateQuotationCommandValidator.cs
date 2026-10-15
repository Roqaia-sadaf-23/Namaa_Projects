using FluentValidation;namespace Namaa.Application.Quotations.Commands.CreateQuotation;
public sealed class CreateQuotationCommandValidator:AbstractValidator<CreateQuotationCommand>{public CreateQuotationCommandValidator(){RuleFor(x=>x.CustomerId).GreaterThan(0);RuleFor(x=>x.QuotationNumber).NotEmpty().MaximumLength(30);RuleFor(x=>x.Title).NotEmpty().MaximumLength(250);RuleFor(x=>x.ValidUntil).GreaterThanOrEqualTo(x=>x.IssueDate).When(x=>x.ValidUntil.HasValue);RuleFor(x=>x.Notes).MaximumLength(1500);RuleFor(x=>x.Terms).MaximumLength(2000);}}

