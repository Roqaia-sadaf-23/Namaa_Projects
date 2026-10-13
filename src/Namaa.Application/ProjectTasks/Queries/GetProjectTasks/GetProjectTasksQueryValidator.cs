using FluentValidation;

namespace Namaa.Application.ProjectTasks.Queries.GetProjectTasks;

public sealed class GetProjectTasksQueryValidator : AbstractValidator<GetProjectTasksQuery>
{
    public GetProjectTasksQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);

        RuleFor(query => query.StageId)
            .GreaterThan(0)
            .When(query => query.StageId.HasValue);
    }
}
