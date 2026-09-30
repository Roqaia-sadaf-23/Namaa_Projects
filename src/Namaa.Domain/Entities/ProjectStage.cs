namespace Namaa.Domain.Entities;

public class ProjectStage
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int StageOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}
