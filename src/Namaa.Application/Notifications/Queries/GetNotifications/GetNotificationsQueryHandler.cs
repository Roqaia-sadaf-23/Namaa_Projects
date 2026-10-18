using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Errors;
using Namaa.Application.Notifications.Dtos;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Notifications.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler(
    INotificationRepository repository,
    ICurrentUser currentUser)
    : IRequestHandler<GetNotificationsQuery, Result<IReadOnlyList<NotificationDto>>>
{
    public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not long userId)
            return Result.Failure<IReadOnlyList<NotificationDto>>(ApplicationErrors.Unauthenticated);

        var notifications = await repository.GetAllAsync(
            userId,
            request.IsRead,
            cancellationToken);

        return Result.Success<IReadOnlyList<NotificationDto>>(
            notifications.Select(NotificationDto.FromEntity).ToList());
    }
}
