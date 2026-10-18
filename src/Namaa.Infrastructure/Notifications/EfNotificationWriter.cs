using Microsoft.Extensions.Logging;
using Namaa.Application.Abstractions.Notifications;
using Namaa.Domain.Entities;
using Namaa.Infrastructure.Persistence;

namespace Namaa.Infrastructure.Notifications;

public sealed class EfNotificationWriter(
    NamaaDbContext dbContext,
    ILogger<EfNotificationWriter> logger) : INotificationWriter
{
    private static readonly Action<ILogger, long, string, long, string, Exception?> LogPersistenceFailure =
        LoggerMessage.Define<long, string, long, string>(
            LogLevel.Error,
            new EventId(1, "NotificationPersistenceFailure"),
            "Notification persistence failed for recipient {RecipientUserId}, type {NotificationType}, " +
            "entity {EntityId}; failure type {FailureType}");

    public async Task CreateAsync(
        NotificationWriteModel notification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        try
        {
            var entity = Notification.Create(
                notification.RecipientUserId,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.EntityId);

            await dbContext.Notifications.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            LogPersistenceFailure(
                logger,
                notification.RecipientUserId,
                notification.Type,
                notification.EntityId,
                exception.GetType().Name,
                null);
            throw;
        }
    }
}
