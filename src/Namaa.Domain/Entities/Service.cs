namespace Namaa.Domain.Entities;

public class Service
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public decimal DefaultPrice { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    public ICollection<QuotationItem> QuotationItems { get; set; } = new List<QuotationItem>();
}
