namespace Namaa.Domain.Entities;

public class Account
{
    public long Id { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public long? ParentAccountId { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public bool IsPostingAllowed { get; set; }
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Account? ParentAccount { get; set; }
    public ICollection<Account> ChildAccounts { get; set; } = new List<Account>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; set; } = new List<JournalEntryLine>();
}
