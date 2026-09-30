using Namaa.Domain.Enums;

namespace Namaa.Domain.Entities;

public class Payment
{
    private Payment()
    {
    }

    private Payment(
        string paymentNumber,
        DateTime paymentDate,
        PaymentMethod paymentMethod,
        decimal amount,
        long receivedByUserId,
        long? invoiceId,
        long? customerId,
        string? referenceNumber,
        string? notes)
    {
        PaymentNumber = DomainGuard.Required(paymentNumber, nameof(paymentNumber));
        PaymentDate = paymentDate;
        PaymentMethod = DomainGuard.Defined(paymentMethod, nameof(paymentMethod));
        Amount = DomainGuard.Positive(amount, nameof(amount));
        ReceivedByUserId = DomainGuard.Positive(receivedByUserId, nameof(receivedByUserId));
        DomainGuard.OptionalPositive(invoiceId, nameof(invoiceId));
        DomainGuard.OptionalPositive(customerId, nameof(customerId));
        InvoiceId = invoiceId;
        CustomerId = customerId;
        ReferenceNumber = referenceNumber;
        Notes = notes;
        Status = "Draft";
        CreatedAt = DateTime.UtcNow;
    }

    public long Id { get; private set; }
    public long? InvoiceId { get; private set; }
    public string PaymentNumber { get; private set; } = string.Empty;
    public DateTime PaymentDate { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public decimal Amount { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public string? Notes { get; private set; }
    public long ReceivedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    public long? CustomerId { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime? PostedAt { get; private set; }
    public long? PostedByUserId { get; private set; }

    public Invoice? Invoice { get; private set; }
    public Customer? Customer { get; private set; }
    public User ReceivedByUser { get; private set; } = null!;
    public User? PostedByUser { get; private set; }

    public static Payment Create(
        string paymentNumber,
        DateTime paymentDate,
        PaymentMethod paymentMethod,
        decimal amount,
        long receivedByUserId,
        long? invoiceId = null,
        long? customerId = null,
        string? referenceNumber = null,
        string? notes = null) =>
        new(paymentNumber, paymentDate, paymentMethod, amount, receivedByUserId,
            invoiceId, customerId, referenceNumber, notes);

    public void Update(
        DateTime paymentDate,
        PaymentMethod paymentMethod,
        decimal amount,
        string? referenceNumber = null,
        string? notes = null)
    {
        var validatedMethod = DomainGuard.Defined(paymentMethod, nameof(paymentMethod));
        var validatedAmount = DomainGuard.Positive(amount, nameof(amount));
        PaymentDate = paymentDate;
        PaymentMethod = validatedMethod;
        Amount = validatedAmount;
        ReferenceNumber = referenceNumber;
        Notes = notes;
    }

    public void Post(long postedByUserId, DateTime postedAt)
    {
        PostedByUserId = DomainGuard.Positive(postedByUserId, nameof(postedByUserId));
        PostedAt = postedAt;
        Status = "Posted";
    }

    public void Cancel()
    {
        Status = "Cancelled";
    }
}
