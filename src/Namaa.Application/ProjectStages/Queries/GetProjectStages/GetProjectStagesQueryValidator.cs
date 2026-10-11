using FluentValidation;

namespace Namaa.Application.ProjectStages.Queries.GetProjectStages;

public sealed class GetProjectStagesQueryValidator : AbstractValidator<GetProjectStagesQuery>
{
    public GetProjectStagesQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);
    }
}
