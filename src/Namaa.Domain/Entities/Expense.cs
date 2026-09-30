namespace Namaa.Domain.Entities;

public class Expense
{
    public long Id { get; set; }
    public long? ProjectId { get; set; }
    public string ExpenseNumber { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public long CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public Project? Project { get; set; }
    public User CreatedByUser { get; set; } = null!;
}
