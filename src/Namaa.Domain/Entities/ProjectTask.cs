using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class ProjectTask
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long? ParentTaskId { get; set; }
    public long? AssignedToUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectTaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal ProgressPercentage { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public long? StageId { get; set; }
    public string? TaskType { get; set; }

    public Project Project { get; set; } = null!;
    public ProjectStage? Stage { get; set; }
    public ProjectTask? ParentTask { get; set; }
    public ICollection<ProjectTask> ChildTasks { get; set; } = new List<ProjectTask>();
    public User? AssignedToUser { get; set; }
    public User CreatedByUser { get; set; } = null!;
}
