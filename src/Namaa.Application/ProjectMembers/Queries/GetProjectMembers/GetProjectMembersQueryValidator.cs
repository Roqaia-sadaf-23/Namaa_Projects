using FluentValidation;

namespace Namaa.Application.ProjectMembers.Queries.GetProjectMembers;

public sealed class GetProjectMembersQueryValidator : AbstractValidator<GetProjectMembersQuery>
{
    public GetProjectMembersQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);
    }
}
