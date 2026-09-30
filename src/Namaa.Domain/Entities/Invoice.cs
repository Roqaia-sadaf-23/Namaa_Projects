using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class Invoice
{
    public long Id { get; set; }
    public long? ProjectId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; }
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
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public long? CustomerId { get; set; }

    public Project? Project { get; set; }
    public Customer? Customer { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
