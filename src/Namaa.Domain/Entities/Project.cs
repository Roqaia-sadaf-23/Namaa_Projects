using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class Project
{
    private Project()
    {
    }

    private Project(
        long customerId,
        string projectCode,
        string name,
        decimal budgetAmount,
        long createdByUserId,
        long? sourceQuotationId,
        long? projectManagerId,
        string? description,
        string? location,
        DateTime? startDate,
        DateTime? endDate)
    {
        CustomerId = DomainGuard.Positive(customerId, nameof(customerId));
        CreatedByUserId = DomainGuard.Positive(createdByUserId, nameof(createdByUserId));
        DomainGuard.OptionalPositive(sourceQuotationId, nameof(sourceQuotationId));
        DomainGuard.OptionalPositive(projectManagerId, nameof(projectManagerId));
        ProjectCode = DomainGuard.Required(projectCode, nameof(projectCode));
        Name = DomainGuard.Required(name, nameof(name));
        BudgetAmount = DomainGuard.NonNegative(budgetAmount, nameof(budgetAmount));
        DomainGuard.EndNotBeforeStart(startDate, endDate, nameof(endDate));
        SourceQuotationId = sourceQuotationId;
        ProjectManagerId = projectManagerId;
        Description = description;
        Location = location;
        StartDate = startDate;
        EndDate = endDate;
        Status = ProjectStatus.Planned;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long CustomerId { get; private set; }
    public long? SourceQuotationId { get; private set; }
    public long? ProjectManagerId { get; private set; }
    public string ProjectCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Location { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime? ActualCompletionDate { get; private set; }
    public ProjectStatus Status { get; private set; }
    public decimal ProgressPercentage { get; private set; }
    public decimal BudgetAmount { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public Customer Customer { get; private set; } = null!;
    public Quotation? SourceQuotation { get; private set; }
    public User? ProjectManager { get; private set; }
    public User CreatedByUser { get; private set; } = null!;
    public ICollection<ProjectMember> Members { get; private set; } = new List<ProjectMember>();
    public ICollection<ProjectTask> Tasks { get; private set; } = new List<ProjectTask>();
    public ICollection<ProjectFile> Files { get; private set; } = new List<ProjectFile>();
    public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();
    public ICollection<Expense> Expenses { get; private set; } = new List<Expense>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; private set; } = new List<JournalEntryLine>();
    public ICollection<PaymentVoucher> PaymentVouchers { get; private set; } = new List<PaymentVoucher>();
    public ICollection<ProjectCorrespondence> Correspondences { get; private set; } = new List<ProjectCorrespondence>();
    public ICollection<ProjectStage> Stages { get; private set; } = new List<ProjectStage>();
    public ICollection<ProjectStatusHistory> StatusHistoryEntries { get; private set; } = new List<ProjectStatusHistory>();
    public ICollection<PurchaseInvoice> PurchaseInvoices { get; private set; } = new List<PurchaseInvoice>();

    public static Project Create(
        long customerId,
        string projectCode,
        string name,
        decimal budgetAmount,
        long createdByUserId,
        long? sourceQuotationId = null,
        long? projectManagerId = null,
        string? description = null,
        string? location = null,
        DateTime? startDate = null,
        DateTime? endDate = null) =>
        new(customerId, projectCode, name, budgetAmount, createdByUserId, sourceQuotationId,
            projectManagerId, description, location, startDate, endDate);

    public void UpdateDetails(
        string name,
        decimal budgetAmount,
        string? description = null,
        string? location = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        long? projectManagerId = null)
    {
        var validatedName = DomainGuard.Required(name, nameof(name));
        var validatedBudget = DomainGuard.NonNegative(budgetAmount, nameof(budgetAmount));
        DomainGuard.OptionalPositive(projectManagerId, nameof(projectManagerId));
        DomainGuard.EndNotBeforeStart(startDate, endDate, nameof(endDate));
        if (ActualCompletionDate.HasValue)
            DomainGuard.EndNotBeforeStart(startDate, ActualCompletionDate, nameof(ActualCompletionDate));
        Name = validatedName;
        BudgetAmount = validatedBudget;
        Description = description;
        Location = location;
        StartDate = startDate;
        EndDate = endDate;
        ProjectManagerId = projectManagerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProgress(decimal progressPercentage)
    {
        ProgressPercentage = DomainGuard.Percentage(progressPercentage, nameof(progressPercentage));
        UpdatedAt = DateTime.UtcNow;
    }

    public ProjectStatusHistory ChangeStatus(
        ProjectStatus status,
        long changedByUserId,
        string? reason = null,
        DateTime? changedAt = null)
    {
        status = DomainGuard.Defined(status, nameof(status));
        var history = ProjectStatusHistory.Create(
            this, Status.ToString(), status.ToString(), changedByUserId, reason, changedAt ?? DateTime.Now);
        Status = status;
        StatusHistoryEntries.Add(history);
        UpdatedAt = DateTime.UtcNow;
        return history;
    }

    public void Complete(DateTime actualCompletionDate, long changedByUserId, string? reason = null)
    {
        DomainGuard.Positive(changedByUserId, nameof(changedByUserId));
        DomainGuard.EndNotBeforeStart(StartDate, actualCompletionDate, nameof(actualCompletionDate));
        ActualCompletionDate = actualCompletionDate;
        ChangeStatus(ProjectStatus.Completed, changedByUserId, reason);
    }
}
