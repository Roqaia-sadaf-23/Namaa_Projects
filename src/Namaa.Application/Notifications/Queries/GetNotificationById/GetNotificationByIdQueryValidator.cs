using FluentValidation;

namespace Namaa.Application.Notifications.Queries.GetNotificationById;

public sealed class GetNotificationByIdQueryValidator : AbstractValidator<GetNotificationByIdQuery>
{
    public GetNotificationByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}
