using MediatR;
using Namaa.Application.Abstractions.Authentication;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Application.Common.Errors;
using Namaa.Application.Notifications.Dtos;
using Namaa.Domain.Common.Errors;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Notifications.Commands.MarkNotificationRead;

public sealed class MarkNotificationReadCommandHandler(
    INotificationRepository repository,
    ICurrentUser currentUser,
    IApplicationDbContext dbContext)
    : IRequestHandler<MarkNotificationReadCommand, Result<NotificationDto>>
{
    public async Task<Result<NotificationDto>> Handle(
        MarkNotificationReadCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is not long userId)
            return Result.Failure<NotificationDto>(ApplicationErrors.Unauthenticated);

        var notification = await repository.FindForUpdateAsync(userId, request.Id, cancellationToken);
        if (notification is null)
            return Result.Failure<NotificationDto>(NotificationErrors.NotFound);

        if (request.IsRead)
            notification.MarkAsRead();
        else
            notification.MarkAsUnread();

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(NotificationDto.FromEntity(notification));
    }
}
