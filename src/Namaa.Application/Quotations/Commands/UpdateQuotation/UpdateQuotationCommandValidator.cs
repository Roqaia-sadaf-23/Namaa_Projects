using FluentValidation;namespace Namaa.Application.Quotations.Commands.UpdateQuotation;
public sealed class UpdateQuotationCommandValidator:AbstractValidator<UpdateQuotationCommand>{public UpdateQuotationCommandValidator(){RuleFor(x=>x.Id).GreaterThan(0);RuleFor(x=>x.Title).NotEmpty().MaximumLength(250);RuleFor(x=>x.ValidUntil).GreaterThanOrEqualTo(x=>x.IssueDate).When(x=>x.ValidUntil.HasValue);RuleFor(x=>x.Notes).MaximumLength(1500);RuleFor(x=>x.Terms).MaximumLength(2000);}}

