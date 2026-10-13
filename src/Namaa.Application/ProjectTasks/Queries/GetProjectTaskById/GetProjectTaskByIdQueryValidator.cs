using FluentValidation;

namespace Namaa.Application.ProjectTasks.Queries.GetProjectTaskById;

public sealed class GetProjectTaskByIdQueryValidator : AbstractValidator<GetProjectTaskByIdQuery>
{
    public GetProjectTaskByIdQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);
        RuleFor(query => query.Id).GreaterThan(0);
    }
}
