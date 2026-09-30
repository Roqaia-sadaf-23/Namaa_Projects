namespace Namaa.Domain.Entities;

public class ProjectCorrespondence
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string CorrespondenceType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
    public DateTime CorrespondenceDate { get; set; }
    public string? Notes { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
}
