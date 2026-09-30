namespace Namaa.Domain.Entities;

public class CostCenter
{
    public long Id { get; set; }
    public string CostCenterCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long? ParentCostCenterId { get; set; }
    public long? BranchId { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public CostCenter? ParentCostCenter { get; set; }
    public ICollection<CostCenter> ChildCostCenters { get; set; } = new List<CostCenter>();
    public Branch? Branch { get; set; }
    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
