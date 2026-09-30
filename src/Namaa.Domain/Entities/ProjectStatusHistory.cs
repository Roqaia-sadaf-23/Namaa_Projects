namespace Namaa.Domain.Entities;

public class ProjectStatusHistory
{
    private ProjectStatusHistory()
    {
    }

    private ProjectStatusHistory(
        Project project,
        string? oldStatus,
        string newStatus,
        long changedByUserId,
        string? reason,
        DateTime changedAt)
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
        OldStatus = oldStatus;
        NewStatus = DomainGuard.Required(newStatus, nameof(newStatus));
        ChangedByUserId = DomainGuard.Positive(changedByUserId, nameof(changedByUserId));
        Reason = reason;
        ChangedAt = changedAt;
    }

    public long Id { get; private set; }
    public long ProjectId { get; private set; }
    public string? OldStatus { get; private set; }
    public string NewStatus { get; private set; } = string.Empty;
    public string? Reason { get; private set; }
    public long ChangedByUserId { get; private set; }
    public DateTime ChangedAt { get; private set; }

    public Project Project { get; private set; } = null!;
    public User ChangedByUser { get; private set; } = null!;

    internal static ProjectStatusHistory Create(
        Project project,
        string? oldStatus,
        string newStatus,
        long changedByUserId,
        string? reason,
        DateTime changedAt) =>
        new(project, oldStatus, newStatus, changedByUserId, reason, changedAt);
}
