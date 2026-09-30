namespace Namaa.Domain.Entities;

public class Account
{
    private Account()
    {
    }

    private Account(
        string accountCode,
        string accountName,
        string accountType,
        bool isPostingAllowed,
        string? description,
        long? parentAccountId)
    {
        AccountCode = DomainGuard.Required(accountCode, nameof(accountCode));
        AccountName = DomainGuard.Required(accountName, nameof(accountName));
        AccountType = ValidateAccountType(accountType);
        DomainGuard.OptionalPositive(parentAccountId, nameof(parentAccountId));
        ParentAccountId = parentAccountId;
        IsPostingAllowed = isPostingAllowed;
        IsActive = true;
        Description = description;
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public string AccountCode { get; private set; } = string.Empty;
    public string AccountName { get; private set; } = string.Empty;
    public long? ParentAccountId { get; private set; }
    public string AccountType { get; private set; } = string.Empty;
    public bool IsPostingAllowed { get; private set; }
    public bool IsActive { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public Account? ParentAccount { get; private set; }
    public ICollection<Account> ChildAccounts { get; private set; } = new List<Account>();
    public ICollection<JournalEntryLine> JournalEntryLines { get; private set; } = new List<JournalEntryLine>();

    public static Account Create(
        string accountCode,
        string accountName,
        string accountType,
        bool isPostingAllowed = true,
        string? description = null,
        long? parentAccountId = null) =>
        new(accountCode, accountName, accountType, isPostingAllowed, description, parentAccountId);

    public void Update(string accountName, string accountType, bool isPostingAllowed, string? description = null)
    {
        var validatedName = DomainGuard.Required(accountName, nameof(accountName));
        var validatedType = ValidateAccountType(accountType);
        AccountName = validatedName;
        AccountType = validatedType;
        IsPostingAllowed = isPostingAllowed;
        Description = description;
        UpdatedAt = DateTime.Now;
    }

    public void MoveUnder(long? parentAccountId)
    {
        DomainGuard.OptionalPositive(parentAccountId, nameof(parentAccountId));

        if (parentAccountId == Id && Id > 0)
            throw new InvalidOperationException("An account cannot be its own parent.");

        ParentAccountId = parentAccountId;
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

    private static string ValidateAccountType(string accountType) =>
        DomainGuard.OneOf(accountType, nameof(accountType), "Asset", "Liability", "Equity", "Revenue", "Expense");
}
