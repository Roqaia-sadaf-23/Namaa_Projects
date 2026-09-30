namespace Namaa.Domain.Entities;

public class JournalEntryLine
{
    private JournalEntryLine()
    {
    }

    private JournalEntryLine(
        JournalEntry journalEntry,
        short lineNumber,
        long accountId,
        decimal debit,
        decimal credit,
        long? branchId,
        long? costCenterId,
        long? projectId,
        string? description)
    {
        JournalEntry = journalEntry ?? throw new ArgumentNullException(nameof(journalEntry));
        LineNumber = DomainGuard.Positive(lineNumber, nameof(lineNumber));
        AccountId = DomainGuard.Positive(accountId, nameof(accountId));
        DomainGuard.OptionalPositive(branchId, nameof(branchId));
        DomainGuard.OptionalPositive(costCenterId, nameof(costCenterId));
        DomainGuard.OptionalPositive(projectId, nameof(projectId));
        ValidateDebitAndCredit(debit, credit);
        Debit = debit;
        Credit = credit;
        BranchId = branchId;
        CostCenterId = costCenterId;
        ProjectId = projectId;
        Description = description;
    }

    public long Id { get; private set; }
    public long JournalEntryId { get; private set; }
    public short LineNumber { get; private set; }
    public long AccountId { get; private set; }
    public long? BranchId { get; private set; }
    public long? CostCenterId { get; private set; }
    public long? ProjectId { get; private set; }
    public string? Description { get; private set; }
    public decimal Debit { get; private set; }
    public decimal Credit { get; private set; }

    public JournalEntry JournalEntry { get; private set; } = null!;
    public Account Account { get; private set; } = null!;
    public Branch? Branch { get; private set; }
    public CostCenter? CostCenter { get; private set; }
    public Project? Project { get; private set; }

    internal static JournalEntryLine Create(
        JournalEntry journalEntry,
        short lineNumber,
        long accountId,
        decimal debit,
        decimal credit,
        long? branchId,
        long? costCenterId,
        long? projectId,
        string? description) =>
        new(journalEntry, lineNumber, accountId, debit, credit, branchId, costCenterId, projectId, description);

    private static void ValidateDebitAndCredit(decimal debit, decimal credit)
    {
        if (!((debit > 0 && credit == 0) || (credit > 0 && debit == 0)))
            throw new ArgumentException("A journal line must contain either a positive debit or a positive credit, but not both.");
    }
}
