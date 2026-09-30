namespace Namaa.Domain.Entities;

public class Supplier
{
    private Supplier()
    {
    }

    private Supplier(string supplierCode, string name)
    {
        SupplierCode = DomainGuard.Required(supplierCode, nameof(supplierCode));
        Name = DomainGuard.Required(name, nameof(name));
        IsActive = true;
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public string SupplierCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? CommercialRegister { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public ICollection<PurchaseInvoice> PurchaseInvoices { get; private set; } = new List<PurchaseInvoice>();
    public ICollection<PaymentVoucher> PaymentVouchers { get; private set; } = new List<PaymentVoucher>();

    public static Supplier Create(string supplierCode, string name) => new(supplierCode, name);

    public void Update(
        string name,
        string? phoneNumber = null,
        string? email = null,
        string? address = null,
        string? city = null,
        string? taxNumber = null,
        string? commercialRegister = null,
        string? notes = null)
    {
        Name = DomainGuard.Required(name, nameof(name));
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        City = city;
        TaxNumber = taxNumber;
        CommercialRegister = commercialRegister;
        Notes = notes;
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
}
