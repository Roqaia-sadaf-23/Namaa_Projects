namespace Namaa.Domain.Entities;

public class PaymentVoucher
{
    private readonly List<PaymentVoucherAllocation> _allocations = new();

    private PaymentVoucher()
    {
    }

    private PaymentVoucher(
        string voucherNumber,
        DateTime voucherDate,
        string paymentMethod,
        decimal amount,
        long paidByUserId,
        long? supplierId,
        string? beneficiaryName,
        long? projectId,
        string? referenceNumber,
        string? notes)
    {
        VoucherNumber = DomainGuard.Required(voucherNumber, nameof(voucherNumber));
        VoucherDate = voucherDate;
        PaymentMethod = ValidatePaymentMethod(paymentMethod);
        Amount = DomainGuard.Positive(amount, nameof(amount));
        PaidByUserId = DomainGuard.Positive(paidByUserId, nameof(paidByUserId));
        DomainGuard.OptionalPositive(supplierId, nameof(supplierId));
        DomainGuard.OptionalPositive(projectId, nameof(projectId));
        EnsureBeneficiary(supplierId, beneficiaryName);
        SupplierId = supplierId;
        BeneficiaryName = beneficiaryName;
        ProjectId = projectId;
        ReferenceNumber = referenceNumber;
        Notes = notes;
        Status = "Draft";
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public string VoucherNumber { get; private set; } = string.Empty;
    public long? SupplierId { get; private set; }
    public string? BeneficiaryName { get; private set; }
    public long? ProjectId { get; private set; }
    public DateTime VoucherDate { get; private set; }
    public string PaymentMethod { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public long PaidByUserId { get; private set; }
    public long? PostedByUserId { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public Supplier? Supplier { get; private set; }
    public Project? Project { get; private set; }
    public User PaidByUser { get; private set; } = null!;
    public User? PostedByUser { get; private set; }
    public IReadOnlyCollection<PaymentVoucherAllocation> Allocations => _allocations;

    public static PaymentVoucher Create(
        string voucherNumber,
        DateTime voucherDate,
        string paymentMethod,
        decimal amount,
        long paidByUserId,
        long? supplierId = null,
        string? beneficiaryName = null,
        long? projectId = null,
        string? referenceNumber = null,
        string? notes = null) =>
        new(voucherNumber, voucherDate, paymentMethod, amount, paidByUserId,
            supplierId, beneficiaryName, projectId, referenceNumber, notes);

    public PaymentVoucherAllocation AddAllocation(long purchaseInvoiceId, decimal allocatedAmount)
    {
        var allocation = PaymentVoucherAllocation.Create(this, purchaseInvoiceId, allocatedAmount);
        _allocations.Add(allocation);
        UpdatedAt = DateTime.Now;
        return allocation;
    }

    public void RemoveAllocation(PaymentVoucherAllocation allocation)
    {
        ArgumentNullException.ThrowIfNull(allocation);
        if (!_allocations.Remove(allocation))
            throw new InvalidOperationException("The allocation does not belong to this payment voucher.");
        UpdatedAt = DateTime.Now;
    }

    public void Update(
        DateTime voucherDate,
        string paymentMethod,
        decimal amount,
        long? supplierId = null,
        string? beneficiaryName = null,
        long? projectId = null,
        string? referenceNumber = null,
        string? notes = null)
    {
        DomainGuard.OptionalPositive(supplierId, nameof(supplierId));
        DomainGuard.OptionalPositive(projectId, nameof(projectId));
        EnsureBeneficiary(supplierId, beneficiaryName);
        var validatedMethod = ValidatePaymentMethod(paymentMethod);
        var validatedAmount = DomainGuard.Positive(amount, nameof(amount));
        VoucherDate = voucherDate;
        PaymentMethod = validatedMethod;
        Amount = validatedAmount;
        SupplierId = supplierId;
        BeneficiaryName = beneficiaryName;
        ProjectId = projectId;
        ReferenceNumber = referenceNumber;
        Notes = notes;
        UpdatedAt = DateTime.Now;
    }

    public void Post(long postedByUserId, DateTime postedAt)
    {
        PostedByUserId = DomainGuard.Positive(postedByUserId, nameof(postedByUserId));
        PostedAt = postedAt;
        Status = "Posted";
        UpdatedAt = DateTime.Now;
    }

    public void Cancel()
    {
        Status = "Cancelled";
        UpdatedAt = DateTime.Now;
    }

    private static string ValidatePaymentMethod(string paymentMethod) =>
        DomainGuard.OneOf(
            paymentMethod, nameof(paymentMethod), "Cash", "BankTransfer", "Card", "Cheque", "Other");

    private static void EnsureBeneficiary(long? supplierId, string? beneficiaryName)
    {
        if (!supplierId.HasValue && string.IsNullOrWhiteSpace(beneficiaryName))
            throw new ArgumentException("Either a supplier or beneficiary name is required.");
    }
}
