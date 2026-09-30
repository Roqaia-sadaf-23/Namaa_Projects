namespace Namaa.Domain.Entities;

public class Expense
{
    private Expense()
    {
    }

    private Expense(
        string expenseNumber,
        DateTime expenseDate,
        string category,
        string description,
        decimal amount,
        long createdByUserId,
        long? projectId)
    {
        ExpenseNumber = DomainGuard.Required(expenseNumber, nameof(expenseNumber));
        ExpenseDate = expenseDate;
        Category = DomainGuard.Required(category, nameof(category));
        Description = DomainGuard.Required(description, nameof(description));
        Amount = DomainGuard.Positive(amount, nameof(amount));
        CreatedByUserId = DomainGuard.Positive(createdByUserId, nameof(createdByUserId));
        DomainGuard.OptionalPositive(projectId, nameof(projectId));
        ProjectId = projectId;
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long? ProjectId { get; private set; }
    public string ExpenseNumber { get; private set; } = string.Empty;
    public DateTime ExpenseDate { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public long CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    public Project? Project { get; private set; }
    public User CreatedByUser { get; private set; } = null!;

    public static Expense Create(
        string expenseNumber,
        DateTime expenseDate,
        string category,
        string description,
        decimal amount,
        long createdByUserId,
        long? projectId = null) =>
        new(expenseNumber, expenseDate, category, description, amount, createdByUserId, projectId);

    public void Update(DateTime expenseDate, string category, string description, decimal amount, long? projectId = null)
    {
        var validatedCategory = DomainGuard.Required(category, nameof(category));
        var validatedDescription = DomainGuard.Required(description, nameof(description));
        var validatedAmount = DomainGuard.Positive(amount, nameof(amount));
        DomainGuard.OptionalPositive(projectId, nameof(projectId));
        ExpenseDate = expenseDate;
        Category = validatedCategory;
        Description = validatedDescription;
        Amount = validatedAmount;
        ProjectId = projectId;
        UpdatedAt = DateTime.UtcNow;
    }
}
