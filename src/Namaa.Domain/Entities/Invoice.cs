using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class Invoice
{
    private readonly List<InvoiceItem> _items = new();

    private Invoice()
    {
    }

    private Invoice(
        string invoiceNumber,
        DateTime issueDate,
        DateTime? dueDate,
        long createdByUserId,
        long? projectId,
        long? customerId,
        string? notes)
    {
        InvoiceNumber = DomainGuard.Required(invoiceNumber, nameof(invoiceNumber));
        CreatedByUserId = DomainGuard.Positive(createdByUserId, nameof(createdByUserId));
        DomainGuard.OptionalPositive(projectId, nameof(projectId));
        DomainGuard.OptionalPositive(customerId, nameof(customerId));
        DomainGuard.EndNotBeforeStart(issueDate, dueDate, nameof(dueDate));
        IssueDate = issueDate;
        DueDate = dueDate;
        ProjectId = projectId;
        CustomerId = customerId;
        Notes = notes;
        Status = InvoiceStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long? ProjectId { get; private set; }
    public string InvoiceNumber { get; private set; } = string.Empty;
    public InvoiceStatus Status { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime? DueDate { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    public long? CustomerId { get; private set; }

    public Project? Project { get; private set; }
    public Customer? Customer { get; private set; }
    public User CreatedByUser { get; private set; } = null!;
    public IReadOnlyCollection<InvoiceItem> Items => _items;
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    public static Invoice Create(
        string invoiceNumber,
        DateTime issueDate,
        long createdByUserId,
        DateTime? dueDate = null,
        long? projectId = null,
        long? customerId = null,
        string? notes = null) =>
        new(invoiceNumber, issueDate, dueDate, createdByUserId, projectId, customerId, notes);

    public InvoiceItem AddItem(
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount = 0,
        decimal taxAmount = 0,
        long? serviceId = null)
    {
        var lineNumber = Items.Count == 0
            ? (short)1
            : checked((short)(Items.Max(item => item.LineNumber) + 1));
        var item = InvoiceItem.Create(
            this, lineNumber, description, quantity, unitPrice, discountAmount, taxAmount, serviceId);
        _items.Add(item);
        RecalculateTotals();
        return item;
    }

    public void RemoveItem(InvoiceItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_items.Remove(item))
            throw new InvalidOperationException("The item does not belong to this invoice.");

        RecalculateTotals();
    }

    public void UpdateDetails(DateTime issueDate, DateTime? dueDate, string? notes = null)
    {
        DomainGuard.EndNotBeforeStart(issueDate, dueDate, nameof(dueDate));
        IssueDate = issueDate;
        DueDate = dueDate;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Issue() => ChangeStatus(InvoiceStatus.Issued);

    public void MarkAsPartiallyPaid() => ChangeStatus(InvoiceStatus.PartiallyPaid);

    public void MarkAsPaid() => ChangeStatus(InvoiceStatus.Paid);

    public void Cancel() => ChangeStatus(InvoiceStatus.Cancelled);

    private void ChangeStatus(InvoiceStatus status)
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
}
