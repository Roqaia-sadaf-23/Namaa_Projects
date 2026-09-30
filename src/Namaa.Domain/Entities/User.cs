namespace Namaa.Domain.Entities;

public class User
{
    private User()
    {
    }

    private User(byte roleId, string fullName, string? email, string? phoneNumber, string passwordHash)
    {
        RoleId = DomainGuard.Positive(roleId, nameof(roleId));
        FullName = DomainGuard.Required(fullName, nameof(fullName));
        EnsureContactMethod(email, phoneNumber);
        Email = email;
        PhoneNumber = phoneNumber;
        PasswordHash = DomainGuard.Required(passwordHash, nameof(passwordHash));
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public byte RoleId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public Role Role { get; private set; } = null!;
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public ICollection<Quotation> ApprovedQuotations { get; private set; } = new List<Quotation>();
    public ICollection<Quotation> CreatedQuotations { get; private set; } = new List<Quotation>();
    public ICollection<ProjectMember> ProjectMemberships { get; private set; } = new List<ProjectMember>();
    public ICollection<Project> ManagedProjects { get; private set; } = new List<Project>();
    public ICollection<Project> CreatedProjects { get; private set; } = new List<Project>();
    public ICollection<ProjectTask> AssignedTasks { get; private set; } = new List<ProjectTask>();
    public ICollection<ProjectTask> CreatedTasks { get; private set; } = new List<ProjectTask>();
    public ICollection<ProjectFile> ProjectFiles { get; private set; } = new List<ProjectFile>();
    public ICollection<Invoice> CreatedInvoices { get; private set; } = new List<Invoice>();
    public ICollection<Payment> PaymentsReceived { get; private set; } = new List<Payment>();
    public ICollection<Payment> PostedPayments { get; private set; } = new List<Payment>();
    public ICollection<Expense> CreatedExpenses { get; private set; } = new List<Expense>();
    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; private set; } = new List<AuditLog>();
    public ICollection<JournalEntry> CreatedJournalEntries { get; private set; } = new List<JournalEntry>();
    public ICollection<JournalEntry> PostedJournalEntries { get; private set; } = new List<JournalEntry>();
    public ICollection<PaymentVoucher> PaidPaymentVouchers { get; private set; } = new List<PaymentVoucher>();
    public ICollection<PaymentVoucher> PostedPaymentVouchers { get; private set; } = new List<PaymentVoucher>();
    public ICollection<ProjectCorrespondence> CreatedProjectCorrespondences { get; private set; } = new List<ProjectCorrespondence>();
    public ICollection<ProjectStatusHistory> ProjectStatusChanges { get; private set; } = new List<ProjectStatusHistory>();
    public ICollection<PurchaseInvoice> CreatedPurchaseInvoices { get; private set; } = new List<PurchaseInvoice>();
    public ICollection<PurchaseInvoice> PostedPurchaseInvoices { get; private set; } = new List<PurchaseInvoice>();

    public static User Create(byte roleId, string fullName, string? email, string? phoneNumber, string passwordHash) =>
        new(roleId, fullName, email, phoneNumber, passwordHash);

    public void UpdateProfile(string fullName, string? email, string? phoneNumber)
    {
        var validatedName = DomainGuard.Required(fullName, nameof(fullName));
        EnsureContactMethod(email, phoneNumber);
        FullName = validatedName;
        Email = email;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeRole(byte roleId)
    {
        RoleId = DomainGuard.Positive(roleId, nameof(roleId));
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePasswordHash(string passwordHash)
    {
        PasswordHash = DomainGuard.Required(passwordHash, nameof(passwordHash));
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void EnsureContactMethod(string? email, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Either an email address or phone number is required.");
    }
}
