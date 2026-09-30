namespace Namaa.Domain.Entities;

public class PaymentVoucherAllocation
{
    public long Id { get; set; }
    public long PaymentVoucherId { get; set; }
    public long PurchaseInvoiceId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    public PaymentVoucher PaymentVoucher { get; set; } = null!;
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;
}
