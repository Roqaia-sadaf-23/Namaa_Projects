namespace Namaa.Domain.Entities;

public class Customer
{
    private readonly List<CustomerContact> _contacts = new();

    private Customer()
    {
    }

    private Customer(string customerCode, string name, string customerType)
    {
        CustomerCode = DomainGuard.Required(customerCode, nameof(customerCode));
        Name = DomainGuard.Required(name, nameof(name));
        CustomerType = DomainGuard.OneOf(customerType, nameof(customerType), "Company", "Individual");
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public string CustomerCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string CustomerType { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? TaxNumber { get; private set; }
    public string? CommercialRegister { get; private set; }
    public string? Source { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public IReadOnlyCollection<CustomerContact> Contacts => _contacts;
    public ICollection<Quotation> Quotations { get; private set; } = new List<Quotation>();
    public ICollection<Project> Projects { get; private set; } = new List<Project>();
    public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();
    public ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    public static Customer Create(string customerCode, string name, string customerType = "Company") =>
        new(customerCode, name, customerType);

    public void Update(
        string name,
        string customerType,
        string? email = null,
        string? phoneNumber = null,
        string? address = null,
        string? city = null,
        string? taxNumber = null,
        string? commercialRegister = null,
        string? source = null,
        string? notes = null)
    {
        var validatedName = DomainGuard.Required(name, nameof(name));
        var validatedType = DomainGuard.OneOf(customerType, nameof(customerType), "Company", "Individual");
        Name = validatedName;
        CustomerType = validatedType;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        City = city;
        TaxNumber = taxNumber;
        CommercialRegister = commercialRegister;
        Source = source;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public CustomerContact AddContact(
        string fullName,
        string? email,
        string? phoneNumber,
        string? jobTitle = null,
        bool isPrimary = false)
    {
        if (isPrimary && _contacts.Any(contact => contact.IsPrimary))
            throw new InvalidOperationException("Customer already has a primary contact.");

        var contact = CustomerContact.Create(this, fullName, email, phoneNumber, jobTitle, isPrimary);
        _contacts.Add(contact);
        return contact;
    }

    public void SetPrimaryContact(CustomerContact contact)
    {
        ArgumentNullException.ThrowIfNull(contact);
        if (!_contacts.Contains(contact))
            throw new InvalidOperationException("The contact does not belong to this customer.");

        foreach (var existingContact in _contacts)
            existingContact.SetPrimary(existingContact == contact);
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
