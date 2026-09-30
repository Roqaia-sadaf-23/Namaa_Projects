using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class Quotation
{
    private readonly List<QuotationItem> _items = new();
    private readonly List<QuotationPaymentTerm> _paymentTerms = new();

    private Quotation()
    {
    }

    private Quotation(
        long customerId,
        string quotationNumber,
        string title,
        DateTime issueDate,
        DateTime? validUntil,
        long createdByUserId,
        string? notes,
        string? terms)
    {
        CustomerId = DomainGuard.Positive(customerId, nameof(customerId));
        CreatedByUserId = DomainGuard.Positive(createdByUserId, nameof(createdByUserId));
        QuotationNumber = DomainGuard.Required(quotationNumber, nameof(quotationNumber));
        Title = DomainGuard.Required(title, nameof(title));
        DomainGuard.EndNotBeforeStart(issueDate, validUntil, nameof(validUntil));
        IssueDate = issueDate;
        ValidUntil = validUntil;
        Notes = notes;
        Terms = terms;
        Status = QuotationStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long CustomerId { get; private set; }
    public string QuotationNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public QuotationStatus Status { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime? ValidUntil { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public string? Terms { get; private set; }
    public DateTime? SentAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public long? ApprovedByUserId { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public Customer Customer { get; private set; } = null!;
    public IReadOnlyCollection<QuotationItem> Items => _items;
    public IReadOnlyCollection<QuotationPaymentTerm> PaymentTerms => _paymentTerms;
    public User? ApprovedByUser { get; private set; }
    public User CreatedByUser { get; private set; } = null!;
    public Project? Project { get; private set; }

    public static Quotation Create(
        long customerId,
        string quotationNumber,
        string title,
        DateTime issueDate,
        long createdByUserId,
        DateTime? validUntil = null,
        string? notes = null,
        string? terms = null) =>
        new(customerId, quotationNumber, title, issueDate, validUntil, createdByUserId, notes, terms);

    public QuotationItem AddItem(
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount = 0,
        decimal taxAmount = 0,
        long? serviceId = null)
    {
        DomainGuard.OptionalPositive(serviceId, nameof(serviceId));
        var lineNumber = NextItemLineNumber();
        var item = QuotationItem.Create(
            this, lineNumber, description, quantity, unitPrice, discountAmount, taxAmount, serviceId);
        _items.Add(item);
        RecalculateTotals();
        return item;
    }

    public void RemoveItem(QuotationItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_items.Remove(item))
            throw new InvalidOperationException("The item does not belong to this quotation.");

        RecalculateTotals();
    }

    public QuotationPaymentTerm AddPaymentTerm(
        decimal? percentage,
        decimal? amount,
        string? description = null,
        int? dueAfterDays = null,
        DateTime? dueDate = null)
    {
        var installmentNumber = PaymentTerms.Count == 0
            ? (short)1
            : checked((short)(PaymentTerms.Max(term => term.InstallmentNumber) + 1));
        var paymentTerm = QuotationPaymentTerm.Create(
            this, installmentNumber, percentage, amount, description, dueAfterDays, dueDate);
        _paymentTerms.Add(paymentTerm);
        UpdatedAt = DateTime.UtcNow;
        return paymentTerm;
    }

    public void UpdateDetails(
        string title,
        DateTime issueDate,
        DateTime? validUntil,
        string? notes = null,
        string? terms = null)
    {
        var validatedTitle = DomainGuard.Required(title, nameof(title));
        DomainGuard.EndNotBeforeStart(issueDate, validUntil, nameof(validUntil));
        Title = validatedTitle;
        IssueDate = issueDate;
        ValidUntil = validUntil;
        Notes = notes;
        Terms = terms;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSent(DateTime sentAt)
    {
        Status = QuotationStatus.Sent;
        SentAt = sentAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(long approvedByUserId, DateTime approvedAt)
    {
        ApprovedByUserId = DomainGuard.Positive(approvedByUserId, nameof(approvedByUserId));
        Status = QuotationStatus.Approved;
        ApprovedAt = approvedAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(DateTime rejectedAt)
    {
        Status = QuotationStatus.Rejected;
        RejectedAt = rejectedAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsExpired() => ChangeStatus(QuotationStatus.Expired);

    public void MarkAsConvertedToProject() => ChangeStatus(QuotationStatus.ConvertedToProject);

    public void Cancel() => ChangeStatus(QuotationStatus.Cancelled);

    private void ChangeStatus(QuotationStatus status)
    {
        Status = DomainGuard.Defined(status, nameof(status));
        UpdatedAt = DateTime.UtcNow;
    }

    private void RecalculateTotals()
    {
        SubTotal = Items.Sum(item => item.Quantity * item.UnitPrice);
        DiscountAmount = Items.Sum(item => item.DiscountAmount);
        TaxAmount = Items.Sum(item => item.TaxAmount);
        TotalAmount = SubTotal - DiscountAmount + TaxAmount;
        UpdatedAt = DateTime.UtcNow;
    }

    private short NextItemLineNumber() =>
        Items.Count == 0
            ? (short)1
            : checked((short)(Items.Max(item => item.LineNumber) + 1));
}
