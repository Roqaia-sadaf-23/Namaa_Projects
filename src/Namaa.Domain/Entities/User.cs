namespace Namaa.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public byte RoleId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Role Role { get; set; } = null!;
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Quotation> ApprovedQuotations { get; set; } = new List<Quotation>();
    public ICollection<Quotation> CreatedQuotations { get; set; } = new List<Quotation>();
    public ICollection<ProjectMember> ProjectMemberships { get; set; } = new List<ProjectMember>();
    public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();
    public ICollection<Project> CreatedProjects { get; set; } = new List<Project>();
    public ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectTask> CreatedTasks { get; set; } = new List<ProjectTask>();
    public ICollection<ProjectFile> ProjectFiles { get; set; } = new List<ProjectFile>();
    public ICollection<Invoice> CreatedInvoices { get; set; } = new List<Invoice>();
    public ICollection<Payment> PaymentsReceived { get; set; } = new List<Payment>();
    public ICollection<Payment> PostedPayments { get; set; } = new List<Payment>();
    public ICollection<Expense> CreatedExpenses { get; set; } = new List<Expense>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
    public ICollection<JournalEntry> CreatedJournalEntries { get; set; } = new List<JournalEntry>();
    public ICollection<JournalEntry> PostedJournalEntries { get; set; } = new List<JournalEntry>();
    public ICollection<PaymentVoucher> PaidPaymentVouchers { get; set; } = new List<PaymentVoucher>();
    public ICollection<PaymentVoucher> PostedPaymentVouchers { get; set; } = new List<PaymentVoucher>();
    public ICollection<ProjectCorrespondence> CreatedProjectCorrespondences { get; set; } = new List<ProjectCorrespondence>();
    public ICollection<ProjectStatusHistory> ProjectStatusChanges { get; set; } = new List<ProjectStatusHistory>();
    public ICollection<PurchaseInvoice> CreatedPurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
    public ICollection<PurchaseInvoice> PostedPurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
}
