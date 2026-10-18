namespace Namaa.Application.Abstractions.Notifications;

public interface INotificationDeliveryProvider
{
    string Channel { get; }

    Task DeliverAsync(
        ExternalNotificationDelivery notification,
        CancellationToken cancellationToken = default);
}

public sealed record ExternalNotificationDelivery(
    long NotificationId,
    long RecipientUserId,
    string Type,
    string Title,
    string Message);
