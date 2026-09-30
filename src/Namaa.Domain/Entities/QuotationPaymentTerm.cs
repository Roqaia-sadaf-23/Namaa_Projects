namespace Namaa.Domain.Entities;

public class QuotationPaymentTerm
{
    public long Id { get; set; }
    public long QuotationId { get; set; }
    public short InstallmentNumber { get; set; }
    public string? Description { get; set; }
    public decimal? Percentage { get; set; }
    public decimal? Amount { get; set; }
    public int? DueAfterDays { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public Quotation Quotation { get; set; } = null!;
}
