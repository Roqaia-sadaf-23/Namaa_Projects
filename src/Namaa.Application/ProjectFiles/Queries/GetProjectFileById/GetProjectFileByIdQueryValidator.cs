using FluentValidation;

namespace Namaa.Application.ProjectFiles.Queries.GetProjectFileById;

public sealed class GetProjectFileByIdQueryValidator : AbstractValidator<GetProjectFileByIdQuery>
{
    public GetProjectFileByIdQueryValidator()
    {
        RuleFor(query => query.ProjectId).GreaterThan(0);
        RuleFor(query => query.Id).GreaterThan(0);
    }
}
