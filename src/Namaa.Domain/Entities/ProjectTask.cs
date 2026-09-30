using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class ProjectTask
{
    private ProjectTask()
    {
    }

    private ProjectTask(
        long projectId,
        string title,
        long createdByUserId,
        TaskPriority priority,
        string? description,
        DateTime? startDate,
        DateTime? dueDate,
        long? assignedToUserId,
        long? parentTaskId,
        long? stageId,
        string? taskType)
    {
        ProjectId = DomainGuard.Positive(projectId, nameof(projectId));
        CreatedByUserId = DomainGuard.Positive(createdByUserId, nameof(createdByUserId));
        Title = DomainGuard.Required(title, nameof(title));
        Priority = DomainGuard.Defined(priority, nameof(priority));
        DomainGuard.EndNotBeforeStart(startDate, dueDate, nameof(dueDate));
        DomainGuard.OptionalPositive(assignedToUserId, nameof(assignedToUserId));
        DomainGuard.OptionalPositive(parentTaskId, nameof(parentTaskId));
        DomainGuard.OptionalPositive(stageId, nameof(stageId));
        Description = description;
        StartDate = startDate;
        DueDate = dueDate;
        AssignedToUserId = assignedToUserId;
        ParentTaskId = parentTaskId;
        StageId = stageId;
        TaskType = ValidateTaskType(taskType);
        Status = ProjectTaskStatus.ToDo;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long ProjectId { get; private set; }
    public long? ParentTaskId { get; private set; }
    public long? AssignedToUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public ProjectTaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public decimal ProgressPercentage { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    public long? StageId { get; private set; }
    public string? TaskType { get; private set; }

    public Project Project { get; private set; } = null!;
    public ProjectStage? Stage { get; private set; }
    public ProjectTask? ParentTask { get; private set; }
    public ICollection<ProjectTask> ChildTasks { get; private set; } = new List<ProjectTask>();
    public User? AssignedToUser { get; private set; }
    public User CreatedByUser { get; private set; } = null!;

    public static ProjectTask Create(
        long projectId,
        string title,
        long createdByUserId,
        TaskPriority priority = TaskPriority.Medium,
        string? description = null,
        DateTime? startDate = null,
        DateTime? dueDate = null,
        long? assignedToUserId = null,
        long? parentTaskId = null,
        long? stageId = null,
        string? taskType = null) =>
        new(projectId, title, createdByUserId, priority, description, startDate, dueDate,
            assignedToUserId, parentTaskId, stageId, taskType);

    public void Update(
        string title,
        TaskPriority priority,
        string? description = null,
        DateTime? startDate = null,
        DateTime? dueDate = null,
        long? assignedToUserId = null,
        long? stageId = null,
        string? taskType = null)
    {
        var validatedTitle = DomainGuard.Required(title, nameof(title));
        var validatedPriority = DomainGuard.Defined(priority, nameof(priority));
        DomainGuard.EndNotBeforeStart(startDate, dueDate, nameof(dueDate));
        DomainGuard.OptionalPositive(assignedToUserId, nameof(assignedToUserId));
        DomainGuard.OptionalPositive(stageId, nameof(stageId));
        var validatedTaskType = ValidateTaskType(taskType);
        Title = validatedTitle;
        Priority = validatedPriority;
        Description = description;
        StartDate = startDate;
        DueDate = dueDate;
        AssignedToUserId = assignedToUserId;
        StageId = stageId;
        TaskType = validatedTaskType;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Start(decimal progressPercentage = 0)
    {
        var validatedProgress = DomainGuard.Percentage(progressPercentage, nameof(progressPercentage));
        Status = ProjectTaskStatus.InProgress;
        CompletedAt = null;
        ProgressPercentage = validatedProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal progressPercentage)
    {
        ProgressPercentage = DomainGuard.Percentage(progressPercentage, nameof(progressPercentage));
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(DateTime completedAt)
    {
        Status = ProjectTaskStatus.Completed;
        CompletedAt = completedAt;
        ProgressPercentage = 100;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ProjectTaskStatus.Cancelled;
        CompletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        Status = ProjectTaskStatus.ToDo;
        CompletedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string? ValidateTaskType(string? taskType) =>
        taskType is null
            ? null
            : DomainGuard.OneOf(taskType, nameof(taskType), "Administrative", "Technical");
}
