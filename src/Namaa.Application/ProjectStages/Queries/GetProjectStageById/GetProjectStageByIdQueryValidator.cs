using FluentValidation;

namespace Namaa.Application.ProjectStages.Queries.GetProjectStageById;

public sealed class GetProjectStageByIdQueryValidator : AbstractValidator<GetProjectStageByIdQuery>
{
    public GetProjectStageByIdQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);
        RuleFor(query => query.Id).GreaterThan(0);
    }
}
