namespace Namaa.Domain.Entities;

public class Branch
{
    public long Id { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public ICollection<CostCenter> CostCenters { get; set; } = new List<CostCenter>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
