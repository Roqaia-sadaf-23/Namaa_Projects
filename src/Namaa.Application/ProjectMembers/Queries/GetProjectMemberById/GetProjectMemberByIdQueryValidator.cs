using FluentValidation;

namespace Namaa.Application.ProjectMembers.Queries.GetProjectMemberById;

public sealed class GetProjectMemberByIdQueryValidator : AbstractValidator<GetProjectMemberByIdQuery>
{
    public GetProjectMemberByIdQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);
        RuleFor(query => query.Id).GreaterThan(0);
    }
}
