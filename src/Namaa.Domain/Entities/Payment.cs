using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class Payment
{
    public long Id { get; set; }
    public long? InvoiceId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public long ReceivedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    public long? CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PostedAt { get; set; }
    public long? PostedByUserId { get; set; }

    public Invoice? Invoice { get; set; }
    public Customer? Customer { get; set; }
    public User ReceivedByUser { get; set; } = null!;
    public User? PostedByUser { get; set; }
}
