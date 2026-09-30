namespace Namaa.Domain.Entities;

public class InvoiceItem
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }
    public short LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public long? ServiceId { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public Service? Service { get; set; }
}
