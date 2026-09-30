namespace Namaa.Domain.Entities;

public class PurchaseInvoice
{
    public long Id { get; set; }
    public long SupplierId { get; set; }
    public long? ProjectId { get; set; }
    public string PurchaseInvoiceNumber { get; set; } = string.Empty;
    public string? SupplierInvoiceNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public long? PostedByUserId { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Supplier Supplier { get; set; } = null!;
    public Project? Project { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public User? PostedByUser { get; set; }
    public ICollection<PurchaseInvoiceItem> Items { get; set; } = new List<PurchaseInvoiceItem>();
    public ICollection<PaymentVoucherAllocation> PaymentVoucherAllocations { get; set; } = new List<PaymentVoucherAllocation>();
}
