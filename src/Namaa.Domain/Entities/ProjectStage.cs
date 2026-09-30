namespace Namaa.Domain.Entities;

public class ProjectStage
{
    private ProjectStage()
    {
    }

    private ProjectStage(
        long projectId,
        string name,
        int stageOrder,
        string status,
        string? description,
        DateTime? startDate,
        DateTime? endDate)
    {
        ProjectId = DomainGuard.Positive(projectId, nameof(projectId));
        Name = DomainGuard.Required(name, nameof(name));
        StageOrder = DomainGuard.NonNegative(stageOrder, nameof(stageOrder));
        Status = DomainGuard.Required(status, nameof(status));
        DomainGuard.EndNotBeforeStart(startDate, endDate, nameof(endDate));
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public long ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int StageOrder { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public decimal ProgressPercentage { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Project Project { get; private set; } = null!;
    public ICollection<ProjectTask> Tasks { get; private set; } = new List<ProjectTask>();

    public static ProjectStage Create(
        long projectId,
        string name,
        int stageOrder,
        string status,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null) =>
        new(projectId, name, stageOrder, status, description, startDate, endDate);

    public void Update(
        string name,
        int stageOrder,
        string status,
        decimal progressPercentage,
        string? description = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var validatedName = DomainGuard.Required(name, nameof(name));
        var validatedOrder = DomainGuard.NonNegative(stageOrder, nameof(stageOrder));
        var validatedStatus = DomainGuard.Required(status, nameof(status));
        var validatedProgress = DomainGuard.Percentage(progressPercentage, nameof(progressPercentage));
        DomainGuard.EndNotBeforeStart(startDate, endDate, nameof(endDate));
        Name = validatedName;
        StageOrder = validatedOrder;
        Status = validatedStatus;
        ProgressPercentage = validatedProgress;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
    }
}
