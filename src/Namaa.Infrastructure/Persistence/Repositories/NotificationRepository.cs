using Microsoft.EntityFrameworkCore;
using Namaa.Application.Abstractions.Persistence;
using Namaa.Domain.Entities;

namespace Namaa.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(NamaaDbContext dbContext) : INotificationRepository
{
    public Task<Notification?> FindByIdAsync(
        long userId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Notifications
            .AsNoTracking()
            .SingleOrDefaultAsync(notification =>
                notification.UserId == userId && notification.Id == id,
                cancellationToken);

    public Task<Notification?> FindForUpdateAsync(
        long userId,
        long id,
        CancellationToken cancellationToken = default) =>
        dbContext.Notifications.SingleOrDefaultAsync(notification =>
            notification.UserId == userId && notification.Id == id,
            cancellationToken);

    public async Task<IReadOnlyList<Notification>> GetAllAsync(
        long userId,
        bool? isRead,
        CancellationToken cancellationToken = default) =>
        await dbContext.Notifications
            .AsNoTracking()
            .Where(notification =>
                notification.UserId == userId &&
                (!isRead.HasValue || notification.IsRead == isRead.Value))
            .OrderByDescending(notification => notification.CreatedAt)
            .ThenByDescending(notification => notification.Id)
            .ToListAsync(cancellationToken);
}
