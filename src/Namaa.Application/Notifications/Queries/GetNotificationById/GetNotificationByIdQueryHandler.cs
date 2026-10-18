using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Errors;
using Namaa.Application.Notifications.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Notifications.Queries.GetNotificationById;

public sealed class GetNotificationByIdQueryHandler(
    INotificationRepository repository,
    ICurrentUser currentUser)
    : IRequestHandler<GetNotificationByIdQuery, Result<NotificationDto>>
{
    public async Task<Result<NotificationDto>> Handle(
        GetNotificationByIdQuery request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not long userId)
            return Result.Failure<NotificationDto>(ApplicationErrors.Unauthenticated);

        var notification = await repository.FindByIdAsync(userId, request.Id, cancellationToken);

        return notification is null
            ? Result.Failure<NotificationDto>(NotificationErrors.NotFound)
            : Result.Success(NotificationDto.FromEntity(notification));
    }
}
