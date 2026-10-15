using FluentValidation;

namespace Namaa.Application.ProjectStatusHistory.Queries.GetProjectStatusHistoryById;

public sealed class GetProjectStatusHistoryByIdQueryValidator : AbstractValidator<GetProjectStatusHistoryByIdQuery>
{
    public GetProjectStatusHistoryByIdQueryValidator() { RuleFor(x => x.ProjectId).GreaterThan(0); RuleFor(x => x.Id).GreaterThan(0); }
}

