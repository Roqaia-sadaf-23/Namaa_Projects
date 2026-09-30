namespace Namaa.Domain.Entities;

public class Supplier
{
    public long Id { get; set; }
    public string SupplierCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? TaxNumber { get; set; }
    public string? CommercialRegister { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; } = new List<PurchaseInvoice>();
    public ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();
}
