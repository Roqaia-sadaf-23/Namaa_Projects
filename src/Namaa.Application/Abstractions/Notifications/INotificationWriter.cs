namespace Namaa.Application.Abstractions.Notifications;

public interface INotificationWriter
{
    Task CreateAsync(
        NotificationWriteModel notification,
        CancellationToken cancellationToken = default);
}

public sealed record NotificationWriteModel(
    long RecipientUserId,
    string Type,
    long EntityId,
    string Title,
    string Message);
