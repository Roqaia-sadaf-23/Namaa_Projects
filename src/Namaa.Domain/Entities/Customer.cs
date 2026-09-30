namespace Namaa.Domain.Entities;

public class Customer
{
    public long Id { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CustomerType { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? TaxNumber { get; set; }
    public string? CommercialRegister { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<CustomerContact> Contacts { get; set; } = new List<CustomerContact>();
    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
