using FluentValidation;

namespace Namaa.Application.ProjectStatusHistory.Queries.GetProjectStatusHistory;

public sealed class GetProjectStatusHistoryQueryValidator : AbstractValidator<GetProjectStatusHistoryQuery>
{
    public GetProjectStatusHistoryQueryValidator() => RuleFor(x => x.ProjectId).GreaterThan(0);
}

