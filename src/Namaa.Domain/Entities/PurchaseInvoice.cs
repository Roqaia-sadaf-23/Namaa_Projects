namespace Namaa.Domain.Entities;

public class PurchaseInvoice
{
    private readonly List<PurchaseInvoiceItem> _items = new();

    private PurchaseInvoice()
    {
    }

    private PurchaseInvoice(
        long supplierId,
        string purchaseInvoiceNumber,
        DateTime issueDate,
        long createdByUserId,
        long? projectId,
        string? supplierInvoiceNumber,
        DateTime? dueDate,
        string? notes)
    {
        SupplierId = DomainGuard.Positive(supplierId, nameof(supplierId));
        PurchaseInvoiceNumber = DomainGuard.Required(purchaseInvoiceNumber, nameof(purchaseInvoiceNumber));
        CreatedByUserId = DomainGuard.Positive(createdByUserId, nameof(createdByUserId));
        DomainGuard.OptionalPositive(projectId, nameof(projectId));
        DomainGuard.EndNotBeforeStart(issueDate, dueDate, nameof(dueDate));
        ProjectId = projectId;
        SupplierInvoiceNumber = supplierInvoiceNumber;
        IssueDate = issueDate;
        DueDate = dueDate;
        Notes = notes;
        Status = "Draft";
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public long SupplierId { get; private set; }
    public long? ProjectId { get; private set; }
    public string PurchaseInvoiceNumber { get; private set; } = string.Empty;
    public string? SupplierInvoiceNumber { get; private set; }
    public string Status { get; private set; } = string.Empty;
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
    public DateTime? PostedAt { get; private set; }
    public long? PostedByUserId { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public Supplier Supplier { get; private set; } = null!;
    public Project? Project { get; private set; }
    public User CreatedByUser { get; private set; } = null!;
    public User? PostedByUser { get; private set; }
    public IReadOnlyCollection<PurchaseInvoiceItem> Items => _items;
    public ICollection<PaymentVoucherAllocation> PaymentVoucherAllocations { get; private set; } = new List<PaymentVoucherAllocation>();

    public static PurchaseInvoice Create(
        long supplierId,
        string purchaseInvoiceNumber,
        DateTime issueDate,
        long createdByUserId,
        long? projectId = null,
        string? supplierInvoiceNumber = null,
        DateTime? dueDate = null,
        string? notes = null) =>
        new(supplierId, purchaseInvoiceNumber, issueDate, createdByUserId,
            projectId, supplierInvoiceNumber, dueDate, notes);

    public PurchaseInvoiceItem AddItem(
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount = 0,
        decimal taxAmount = 0,
        decimal? lineTotal = null)
    {
        var lineNumber = Items.Count == 0
            ? (short)1
            : checked((short)(Items.Max(item => item.LineNumber) + 1));
        var item = PurchaseInvoiceItem.Create(
            this, lineNumber, description, quantity, unitPrice, discountAmount, taxAmount, lineTotal);
        _items.Add(item);
        RecalculateTotals();
        return item;
    }

    public void RemoveItem(PurchaseInvoiceItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!_items.Remove(item))
            throw new InvalidOperationException("The item does not belong to this purchase invoice.");
        RecalculateTotals();
    }

    public void UpdateDetails(
        DateTime issueDate,
        DateTime? dueDate,
        string? supplierInvoiceNumber = null,
        string? notes = null)
    {
        DomainGuard.EndNotBeforeStart(issueDate, dueDate, nameof(dueDate));
        IssueDate = issueDate;
        DueDate = dueDate;
        SupplierInvoiceNumber = supplierInvoiceNumber;
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

    private void RecalculateTotals()
    {
        SubTotal = Items.Sum(item => item.Quantity * item.UnitPrice);
        DiscountAmount = Items.Sum(item => item.DiscountAmount);
        TaxAmount = Items.Sum(item => item.TaxAmount);
        TotalAmount = Items.Sum(item => item.LineTotal);
        UpdatedAt = DateTime.Now;
    }
}
