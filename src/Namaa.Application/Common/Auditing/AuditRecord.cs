namespace Namaa.Application.Common.Auditing;

public sealed record AuditRecord(
    long? ActorUserId,
    string Action,
    string EntityType,
    long EntityId,
    string? NewValues = null);
