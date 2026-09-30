namespace Namaa.Domain.Entities;

public class PaymentVoucher
{
    public long Id { get; set; }
    public string VoucherNumber { get; set; } = string.Empty;
    public long? SupplierId { get; set; }
    public string? BeneficiaryName { get; set; }
    public long? ProjectId { get; set; }
    public DateTime VoucherDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public long PaidByUserId { get; set; }
    public long? PostedByUserId { get; set; }
    public DateTime? PostedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Supplier? Supplier { get; set; }
    public Project? Project { get; set; }
    public User PaidByUser { get; set; } = null!;
    public User? PostedByUser { get; set; }
    public ICollection<PaymentVoucherAllocation> Allocations { get; set; } = new List<PaymentVoucherAllocation>();
}
