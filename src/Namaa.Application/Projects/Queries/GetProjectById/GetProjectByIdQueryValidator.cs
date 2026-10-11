using FluentValidation;

namespace Namaa.Application.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryValidator : AbstractValidator<GetProjectByIdQuery>
{
    public GetProjectByIdQueryValidator()
    {
        RuleFor(query => query.Id).GreaterThan(0);
    }
}
