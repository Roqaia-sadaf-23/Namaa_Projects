using FluentValidation;namespace Namaa.Application.Quotations.Queries.GetQuotationById;public sealed class GetQuotationByIdQueryValidator:AbstractValidator<GetQuotationByIdQuery>{public GetQuotationByIdQueryValidator()=>RuleFor(x=>x.Id).GreaterThan(0);}

