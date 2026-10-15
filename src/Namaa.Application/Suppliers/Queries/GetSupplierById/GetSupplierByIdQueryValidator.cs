using FluentValidation;

namespace Namaa.Application.Suppliers.Queries.GetSupplierById;

public sealed class GetSupplierByIdQueryValidator : AbstractValidator<GetSupplierByIdQuery>
{
    public GetSupplierByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan((long)0);
}

