using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class Project
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public long? SourceQuotationId { get; set; }
    public long? ProjectManagerId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }
    public ProjectStatus Status { get; set; }
    public decimal ProgressPercentage { get; set; }
    public decimal BudgetAmount { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Customer Customer { get; set; } = null!;
    public Quotation? SourceQuotation { get; set; }
    public User? ProjectManager { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectFile> Files { get; set; } = new List<ProjectFile>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
    public ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();
    public ICollection<ProjectCorrespondence> Correspondences { get; set; } = new List<ProjectCorrespondence>();
    public ICollection<ProjectStage> Stages { get; set; } = new List<ProjectStage>();
    public ICollection<ProjectStatusHistory> StatusHistoryEntries { get; set; } = new List<ProjectStatusHistory>();
    public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
}
