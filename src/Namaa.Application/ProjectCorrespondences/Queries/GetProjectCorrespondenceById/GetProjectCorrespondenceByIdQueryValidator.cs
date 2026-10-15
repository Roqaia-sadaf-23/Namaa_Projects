using FluentValidation;

namespace Namaa.Application.ProjectCorrespondences.Queries.GetProjectCorrespondenceById;

public sealed class GetProjectCorrespondenceByIdQueryValidator : AbstractValidator<GetProjectCorrespondenceByIdQuery>
{
    public GetProjectCorrespondenceByIdQueryValidator() { RuleFor(x => x.ProjectId).GreaterThan(0); RuleFor(x => x.Id).GreaterThan(0); }
}

