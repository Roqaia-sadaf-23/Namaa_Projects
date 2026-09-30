namespace Namaa.Domain.Entities;

public class JournalEntry
{
    private readonly List<JournalEntryLine> _lines = new();

    private JournalEntry()
    {
    }

    private JournalEntry(
        string entryNumber,
        DateTime entryDate,
        long createdByUserId,
        string? description,
        string? sourceType,
        long? sourceId)
    {
        EntryNumber = DomainGuard.Required(entryNumber, nameof(entryNumber));
        EntryDate = entryDate;
        CreatedByUserId = DomainGuard.Positive(createdByUserId, nameof(createdByUserId));
        DomainGuard.OptionalPositive(sourceId, nameof(sourceId));
        Description = description;
        SourceType = sourceType;
        SourceId = sourceId;
        Status = "Draft";
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public string EntryNumber { get; private set; } = string.Empty;
    public DateTime EntryDate { get; private set; }
    public string? Description { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string? SourceType { get; private set; }
    public long? SourceId { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public long? PostedByUserId { get; private set; }
    public DateTime? PostedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public User CreatedByUser { get; private set; } = null!;
    public User? PostedByUser { get; private set; }
    public IReadOnlyCollection<JournalEntryLine> Lines => _lines;

    public static JournalEntry Create(
        string entryNumber,
        DateTime entryDate,
        long createdByUserId,
        string? description = null,
        string? sourceType = null,
        long? sourceId = null) =>
        new(entryNumber, entryDate, createdByUserId, description, sourceType, sourceId);

    public JournalEntryLine AddLine(
        long accountId,
        decimal debit,
        decimal credit,
        long? branchId = null,
        long? costCenterId = null,
        long? projectId = null,
        string? description = null)
    {
        var lineNumber = Lines.Count == 0
            ? (short)1
            : checked((short)(Lines.Max(line => line.LineNumber) + 1));
        var line = JournalEntryLine.Create(
            this, lineNumber, accountId, debit, credit, branchId, costCenterId, projectId, description);
        _lines.Add(line);
        UpdatedAt = DateTime.Now;
        return line;
    }

    public void RemoveLine(JournalEntryLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (!_lines.Remove(line))
            throw new InvalidOperationException("The line does not belong to this journal entry.");
        UpdatedAt = DateTime.Now;
    }

    public void Update(DateTime entryDate, string? description = null)
    {
        EntryDate = entryDate;
        Description = description;
        UpdatedAt = DateTime.Now;
    }

    public void Post(long postedByUserId, DateTime postedAt)
    {
        PostedByUserId = DomainGuard.Positive(postedByUserId, nameof(postedByUserId));
        PostedAt = postedAt;
        Status = "Posted";
        UpdatedAt = DateTime.Now;
    }

    public void Cancel()
    {
        Status = "Cancelled";
        UpdatedAt = DateTime.Now;
    }
}
