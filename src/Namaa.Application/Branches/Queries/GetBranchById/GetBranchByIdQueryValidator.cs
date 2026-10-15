using FluentValidation;

namespace Namaa.Application.Branches.Queries.GetBranchById;

public sealed class GetBranchByIdQueryValidator : AbstractValidator<GetBranchByIdQuery>
{
    public GetBranchByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan((long)0);
}

