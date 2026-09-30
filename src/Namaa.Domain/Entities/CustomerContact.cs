namespace Namaa.Domain.Entities;

public class CustomerContact
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }

    public Customer Customer { get; set; } = null!;
}
