using FluentValidation;

namespace Namaa.Application.ProjectCorrespondences.Queries.GetProjectCorrespondences;

public sealed class GetProjectCorrespondencesQueryValidator : AbstractValidator<GetProjectCorrespondencesQuery>
{
    public GetProjectCorrespondencesQueryValidator() => RuleFor(x => x.ProjectId).GreaterThan(0);
}

