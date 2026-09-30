namespace Namaa.Domain.Entities;

public class Service
{
    private Service()
    {
    }

    private Service(string name, decimal defaultPrice, string? category, string? description)
    {
        Name = DomainGuard.Required(name, nameof(name));
        DefaultPrice = DomainGuard.NonNegative(defaultPrice, nameof(defaultPrice));
        Category = category;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Category { get; private set; }
    public string? Description { get; private set; }
    public decimal DefaultPrice { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public ICollection<InvoiceItem> InvoiceItems { get; private set; } = new List<InvoiceItem>();
    public ICollection<QuotationItem> QuotationItems { get; private set; } = new List<QuotationItem>();

    public static Service Create(string name, decimal defaultPrice, string? category = null, string? description = null) =>
        new(name, defaultPrice, category, description);

    public void Update(string name, decimal defaultPrice, string? category = null, string? description = null)
    {
        var validatedName = DomainGuard.Required(name, nameof(name));
        var validatedPrice = DomainGuard.NonNegative(defaultPrice, nameof(defaultPrice));
        Name = validatedName;
        DefaultPrice = validatedPrice;
        Category = category;
        Description = description;
        UpdatedAt = DateTime.Now;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.Now;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.Now;
    }

    public void ChangePrice(decimal defaultPrice)
    {
        DefaultPrice = DomainGuard.NonNegative(defaultPrice, nameof(defaultPrice));
        UpdatedAt = DateTime.Now;
    }
}
