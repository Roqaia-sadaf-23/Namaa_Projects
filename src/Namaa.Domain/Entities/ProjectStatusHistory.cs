namespace Namaa.Domain.Entities;

public class ProjectStatusHistory
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string? OldStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public long ChangedByUserId { get; set; }
    public DateTime ChangedAt { get; set; }

    public Project Project { get; set; } = null!;
    public User ChangedByUser { get; set; } = null!;
}
