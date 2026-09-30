namespace Namaa.Domain.Entities;

public class Notification
{
    private Notification()
    {
    }

    private Notification(long userId, string title, string message, string? entityType, long? entityId)
    {
        UserId = DomainGuard.Positive(userId, nameof(userId));
        Title = DomainGuard.Required(title, nameof(title));
        Message = DomainGuard.Required(message, nameof(message));
        EnsureEntityReference(entityType, entityId);
        EntityType = entityType;
        EntityId = entityId;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string? EntityType { get; private set; }
    public long? EntityId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public User User { get; private set; } = null!;

    public static Notification Create(
        long userId,
        string title,
        string message,
        string? entityType = null,
        long? entityId = null) =>
        new(userId, title, message, entityType, entityId);

    public void MarkAsRead(DateTime? readAt = null)
    {
        IsRead = true;
        ReadAt = readAt ?? DateTime.UtcNow;
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
    }

    private static void EnsureEntityReference(string? entityType, long? entityId)
    {
        if ((entityType is null) != !entityId.HasValue)
            throw new ArgumentException("Entity type and entity ID must either both be provided or both be omitted.");

        DomainGuard.OptionalPositive(entityId, nameof(entityId));
    }
}
