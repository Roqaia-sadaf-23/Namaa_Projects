using Namaa.Domain.Entities;

namespace Namaa.Tests.Domain;

public sealed class NotificationTests
{
    [Fact]
    public void MarkAsRead_WhenAlreadyRead_IsIdempotent()
    {
        var notification = Notification.Create(1, "Title", "Message");
        var firstReadAt = new DateTime(2026, 10, 18, 12, 0, 0, DateTimeKind.Utc);

        notification.MarkAsRead(firstReadAt);
        notification.MarkAsRead(firstReadAt.AddHours(1));

        Assert.True(notification.IsRead);
        Assert.Equal(firstReadAt, notification.ReadAt);
    }

    [Fact]
    public void MarkAsUnread_WhenAlreadyUnread_IsIdempotent()
    {
        var notification = Notification.Create(1, "Title", "Message");

        notification.MarkAsUnread();

        Assert.False(notification.IsRead);
        Assert.Null(notification.ReadAt);
    }
}
