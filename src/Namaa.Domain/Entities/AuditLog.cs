using System.Text.Json;

namespace Namaa.Domain.Entities;

public class AuditLog
{
    private AuditLog()
    {
    }

    private AuditLog(
        string action,
        string entityType,
        long? userId,
        long? entityId,
        string? oldValues,
        string? newValues,
        string? ipAddress)
    {
        Action = DomainGuard.Required(action, nameof(action));
        EntityType = DomainGuard.Required(entityType, nameof(entityType));
        DomainGuard.OptionalPositive(userId, nameof(userId));
        DomainGuard.OptionalPositive(entityId, nameof(entityId));
        EnsureValidJson(oldValues, nameof(oldValues));
        EnsureValidJson(newValues, nameof(newValues));
        UserId = userId;
        EntityId = entityId;
        OldValues = oldValues;
        NewValues = newValues;
        IpAddress = ipAddress;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long? UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public long? EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User? User { get; private set; }

    public static AuditLog Create(
        string action,
        string entityType,
        long? userId = null,
        long? entityId = null,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null) =>
        new(action, entityType, userId, entityId, oldValues, newValues, ipAddress);

    private static void EnsureValidJson(string? value, string parameterName)
    {
        if (value is null)
            return;

        try
        {
            using var _ = JsonDocument.Parse(value);
        }
        catch (JsonException exception)
        {
            throw new ArgumentException("Value must contain valid JSON.", parameterName, exception);
        }
    }
}
