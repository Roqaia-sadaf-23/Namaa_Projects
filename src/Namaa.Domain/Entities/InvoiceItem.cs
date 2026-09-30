namespace Namaa.Domain.Entities;

public class InvoiceItem
{
    private InvoiceItem()
    {
    }

    private InvoiceItem(
        Invoice invoice,
        short lineNumber,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount,
        decimal taxAmount,
        long? serviceId)
    {
        Invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
        LineNumber = DomainGuard.Positive(lineNumber, nameof(lineNumber));
        DomainGuard.OptionalPositive(serviceId, nameof(serviceId));
        ServiceId = serviceId;
        SetAmounts(description, quantity, unitPrice, discountAmount, taxAmount);
    }

    public long Id { get; private set; }
    public long InvoiceId { get; private set; }
    public short LineNumber { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal LineTotal { get; private set; }
    public long? ServiceId { get; private set; }

    public Invoice Invoice { get; private set; } = null!;
    public Service? Service { get; private set; }

    internal static InvoiceItem Create(
        Invoice invoice,
        short lineNumber,
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount,
        decimal taxAmount,
        long? serviceId) =>
        new(invoice, lineNumber, description, quantity, unitPrice, discountAmount, taxAmount, serviceId);

    internal void Update(
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount,
        decimal taxAmount,
        long? serviceId)
    {
        DomainGuard.OptionalPositive(serviceId, nameof(serviceId));
        SetAmounts(description, quantity, unitPrice, discountAmount, taxAmount);
        ServiceId = serviceId;
    }

    private void SetAmounts(
        string description,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount,
        decimal taxAmount)
    {
        var validatedDescription = DomainGuard.Required(description, nameof(description));
        var validatedQuantity = DomainGuard.Positive(quantity, nameof(quantity));
        var validatedUnitPrice = DomainGuard.NonNegative(unitPrice, nameof(unitPrice));
        var validatedDiscount = DomainGuard.NonNegative(discountAmount, nameof(discountAmount));
        var validatedTax = DomainGuard.NonNegative(taxAmount, nameof(taxAmount));

        var grossAmount = validatedQuantity * validatedUnitPrice;
        if (validatedDiscount > grossAmount)
            throw new ArgumentOutOfRangeException(nameof(discountAmount), "Discount cannot exceed the gross line amount.");

        Description = validatedDescription;
        Quantity = validatedQuantity;
        UnitPrice = validatedUnitPrice;
        DiscountAmount = validatedDiscount;
        TaxAmount = validatedTax;
        LineTotal = grossAmount - validatedDiscount + validatedTax;
    }
}
