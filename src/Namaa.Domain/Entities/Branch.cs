namespace Namaa.Domain.Entities;

public class Branch
{
    private Branch()
    {
    }

    private Branch(string branchCode, string name, string? address, string? city)
    {
        BranchCode = DomainGuard.Required(branchCode, nameof(branchCode));
        Name = DomainGuard.Required(name, nameof(name));
        Address = address;
        City = city;
        IsActive = true;
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public string BranchCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public ICollection<CostCenter> CostCenters { get; private set; } = new List<CostCenter>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; private set; } = new List<JournalEntryLine>();

    public static Branch Create(string branchCode, string name, string? address = null, string? city = null) =>
        new(branchCode, name, address, city);

    public void Update(string name, string? address = null, string? city = null)
    {
        Name = DomainGuard.Required(name, nameof(name));
        Address = address;
        City = city;
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
