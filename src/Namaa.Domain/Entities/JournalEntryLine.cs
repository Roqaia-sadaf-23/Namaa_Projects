namespace Namaa.Domain.Entities;

public class JournalEntryLine
{
    public long Id { get; set; }
    public long JournalEntryId { get; set; }
    public short LineNumber { get; set; }
    public long AccountId { get; set; }
    public long? BranchId { get; set; }
    public long? CostCenterId { get; set; }
    public long? ProjectId { get; set; }
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }

    public JournalEntry JournalEntry { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public Branch? Branch { get; set; }
    public CostCenter? CostCenter { get; set; }
    public Project? Project { get; set; }
}
