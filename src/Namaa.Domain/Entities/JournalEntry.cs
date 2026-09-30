namespace Namaa.Domain.Entities;

public class JournalEntry
{
    public long Id { get; set; }
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? SourceType { get; set; }
    public long? SourceId { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? PostedByUserId { get; set; }
    public DateTime? PostedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public User CreatedByUser { get; set; } = null!;
    public User? PostedByUser { get; set; }
    public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
}
