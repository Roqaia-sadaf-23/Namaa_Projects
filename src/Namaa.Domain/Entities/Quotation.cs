using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class Quotation
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string QuotationNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public QuotationStatus Status { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public long? ApprovedByUserId { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Customer Customer { get; set; } = null!;
    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
    public ICollection<QuotationPaymentTerm> PaymentTerms { get; set; } = new List<QuotationPaymentTerm>();
    public User? ApprovedByUser { get; set; }
    public User CreatedByUser { get; set; } = null!;
    public Project? Project { get; set; }
}
