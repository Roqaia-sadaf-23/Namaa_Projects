namespace Namaa.Domain.Entities;

public class PurchaseInvoiceItem
{
    private PurchaseInvoiceItem()
    {
    }

    private PurchaseInvoiceItem(
        PurchaseInvoice purchaseInvoice,
        short lineNumber,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount,
        decimal taxAmount,
        decimal? lineTotal)
    {
        PurchaseInvoice = purchaseInvoice ?? throw new ArgumentNullException(nameof(purchaseInvoice));
        LineNumber = DomainGuard.Positive(lineNumber, nameof(lineNumber));
        SetAmounts(description, quantity, unitPrice, discountAmount, taxAmount, lineTotal);
    }

    public long Id { get; private set; }
    public long PurchaseInvoiceId { get; private set; }
    public short LineNumber { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }

    public PurchaseInvoice PurchaseInvoice { get; private set; } = null!;

    internal static PurchaseInvoiceItem Create(
        PurchaseInvoice purchaseInvoice,
        short lineNumber,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount,
        decimal taxAmount,
        decimal? lineTotal) =>
        new(purchaseInvoice, lineNumber, description, quantity, unitPrice, discountAmount, taxAmount, lineTotal);

    private void SetAmounts(
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount,
        decimal taxAmount,
        decimal? lineTotal)
    {
        Description = DomainGuard.Required(description, nameof(description));
        Quantity = DomainGuard.Positive(quantity, nameof(quantity));
        UnitPrice = DomainGuard.NonNegative(unitPrice, nameof(unitPrice));
        DiscountAmount = DomainGuard.NonNegative(discountAmount, nameof(discountAmount));
        TaxAmount = DomainGuard.NonNegative(taxAmount, nameof(taxAmount));
        LineTotal = DomainGuard.NonNegative(
            lineTotal ?? Quantity * UnitPrice - DiscountAmount + TaxAmount, nameof(lineTotal));
    }
}
