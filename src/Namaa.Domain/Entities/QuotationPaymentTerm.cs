namespace Namaa.Domain.Entities;

public class QuotationPaymentTerm
{
    private QuotationPaymentTerm()
    {
    }

    private QuotationPaymentTerm(
        Quotation quotation,
        short installmentNumber,
        decimal? percentage,
        decimal? amount,
        string? description,
        int? dueAfterDays,
        DateTime? dueDate)
    {
        Quotation = quotation ?? throw new ArgumentNullException(nameof(quotation));
        InstallmentNumber = DomainGuard.Positive(installmentNumber, nameof(installmentNumber));
        SetTerms(percentage, amount, description, dueAfterDays, dueDate);
        CreatedAt = DateTime.Now;
    }

    public long Id { get; private set; }
    public long QuotationId { get; private set; }
    public short InstallmentNumber { get; private set; }
    public string? Description { get; private set; }
    public decimal? Percentage { get; private set; }
    public decimal? Amount { get; private set; }
    public int? DueAfterDays { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Quotation Quotation { get; private set; } = null!;

    internal static QuotationPaymentTerm Create(
        Quotation quotation,
        short installmentNumber,
        decimal? percentage,
        decimal? amount,
        string? description,
        int? dueAfterDays,
        DateTime? dueDate) =>
        new(quotation, installmentNumber, percentage, amount, description, dueAfterDays, dueDate);

    internal void Update(
        decimal? percentage,
        decimal? amount,
        string? description,
        int? dueAfterDays,
        DateTime? dueDate) =>
        SetTerms(percentage, amount, description, dueAfterDays, dueDate);

    private void SetTerms(
        decimal? percentage,
        decimal? amount,
        string? description,
        int? dueAfterDays,
        DateTime? dueDate)
    {
        if (!percentage.HasValue && !amount.HasValue)
            throw new ArgumentException("Either percentage or amount is required.");

        if (percentage.HasValue)
            DomainGuard.Percentage(percentage.Value, nameof(percentage));

        if (amount.HasValue)
            DomainGuard.NonNegative(amount.Value, nameof(amount));

        if (dueAfterDays.HasValue)
            DomainGuard.NonNegative(dueAfterDays.Value, nameof(dueAfterDays));

        Percentage = percentage;
        Amount = amount;
        Description = description;
        DueAfterDays = dueAfterDays;
        DueDate = dueDate;
    }
}
