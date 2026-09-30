namespace Namaa.Domain.Entities;

public class CustomerContact
{
    private CustomerContact()
    {
    }

    private CustomerContact(
        Customer customer,
        string fullName,
        string? email,
        string? phoneNumber,
        string? jobTitle,
        bool isPrimary)
    {
        Customer = customer ?? throw new ArgumentNullException(nameof(customer));
        FullName = DomainGuard.Required(fullName, nameof(fullName));
        EnsureContactMethod(email, phoneNumber);
        Email = email;
        PhoneNumber = phoneNumber;
        JobTitle = jobTitle;
        IsPrimary = isPrimary;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long CustomerId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string? JobTitle { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public bool IsPrimary { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Customer Customer { get; private set; } = null!;

    internal static CustomerContact Create(
        Customer customer,
        string fullName,
        string? email,
        string? phoneNumber,
        string? jobTitle = null,
        bool isPrimary = false) =>
        new(customer, fullName, email, phoneNumber, jobTitle, isPrimary);

    public void Update(string fullName, string? email, string? phoneNumber, string? jobTitle = null)
    {
        var validatedName = DomainGuard.Required(fullName, nameof(fullName));
        EnsureContactMethod(email, phoneNumber);
        FullName = validatedName;
        Email = email;
        PhoneNumber = phoneNumber;
        JobTitle = jobTitle;
    }

    internal void SetPrimary(bool isPrimary) => IsPrimary = isPrimary;

    private static void EnsureContactMethod(string? email, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Either an email address or phone number is required.");
    }
}
