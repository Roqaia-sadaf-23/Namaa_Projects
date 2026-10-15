using FluentValidation;

namespace Namaa.Application.Roles.Queries.GetRoleById;

public sealed class GetRoleByIdQueryValidator : AbstractValidator<GetRoleByIdQuery>
{
    public GetRoleByIdQueryValidator() => RuleFor(x => x.Id).GreaterThan((byte)0);
}

