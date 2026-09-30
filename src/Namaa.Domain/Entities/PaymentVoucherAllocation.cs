namespace Namaa.Domain.Entities;

public class PaymentVoucherAllocation
{
    private PaymentVoucherAllocation()
    {
    }

    private PaymentVoucherAllocation(
        PaymentVoucher paymentVoucher,
        long purchaseInvoiceId,
        decimal allocatedAmount)
    {
        PaymentVoucher = paymentVoucher ?? throw new ArgumentNullException(nameof(paymentVoucher));
        PurchaseInvoiceId = DomainGuard.Positive(purchaseInvoiceId, nameof(purchaseInvoiceId));
        AllocatedAmount = DomainGuard.Positive(allocatedAmount, nameof(allocatedAmount));
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public long PaymentVoucherId { get; private set; }
    public long PurchaseInvoiceId { get; private set; }
    public decimal AllocatedAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PaymentVoucher PaymentVoucher { get; private set; } = null!;
    public PurchaseInvoice PurchaseInvoice { get; private set; } = null!;

    internal static PaymentVoucherAllocation Create(
        PaymentVoucher paymentVoucher,
        long purchaseInvoiceId,
        decimal allocatedAmount) =>
        new(paymentVoucher, purchaseInvoiceId, allocatedAmount);

    internal void ChangeAmount(decimal allocatedAmount) =>
        AllocatedAmount = DomainGuard.Positive(allocatedAmount, nameof(allocatedAmount));
}
