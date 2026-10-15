using FluentValidation;

namespace Namaa.Application.Services.Queries.GetServiceById;

public sealed class GetServiceByIdQueryValidator : AbstractValidator<GetServiceByIdQuery>
{
    public GetServiceByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan((long)0);
}

