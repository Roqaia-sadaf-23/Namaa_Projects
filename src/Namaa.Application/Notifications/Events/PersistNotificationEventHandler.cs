using MediatR;
using Namaa.Application.Abstractions.Notifications;

namespace Namaa.Application.Notifications.Events;

public sealed class PersistNotificationHandler(INotificationWriter writer) :
    INotificationHandler<ProjectTaskAssignedNotificationEvent>,
    INotificationHandler<ProjectMemberAddedNotificationEvent>,
    INotificationHandler<InvoiceIssuedNotificationEvent>
{
    public Task Handle(
        ProjectTaskAssignedNotificationEvent notification,
        CancellationToken cancellationToken) =>
        writer.CreateAsync(NotificationCatalog.For(notification), cancellationToken);

    public Task Handle(
        ProjectMemberAddedNotificationEvent notification,
        CancellationToken cancellationToken) =>
        writer.CreateAsync(NotificationCatalog.For(notification), cancellationToken);

    public Task Handle(
        InvoiceIssuedNotificationEvent notification,
        CancellationToken cancellationToken) =>
        writer.CreateAsync(NotificationCatalog.For(notification), cancellationToken);
}
