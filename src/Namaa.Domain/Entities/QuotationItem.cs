namespace Namaa.Domain.Entities;

public class QuotationItem
{
    public long Id { get; set; }
    public long QuotationId { get; set; }
    public short LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public long? ServiceId { get; set; }

    public Quotation Quotation { get; set; } = null!;
    public Service? Service { get; set; }
}
