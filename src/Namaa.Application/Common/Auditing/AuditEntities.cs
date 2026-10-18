using Namaa.Domain.Entities;

namespace Namaa.Application.Common.Auditing;

public static class AuditEntities
{
    public const string Account = nameof(Account);
    public const string Branch = nameof(Branch);
    public const string CostCenter = nameof(CostCenter);
    public const string Customer = nameof(Customer);
    public const string CustomerContact = nameof(CustomerContact);
    public const string Expense = nameof(Expense);
    public const string Invoice = nameof(Invoice);
    public const string InvoiceItem = nameof(InvoiceItem);
    public const string JournalEntry = nameof(JournalEntry);
    public const string JournalEntryLine = nameof(JournalEntryLine);
    public const string Notification = nameof(Notification);
    public const string Payment = nameof(Payment);
    public const string PaymentVoucher = nameof(PaymentVoucher);
    public const string PaymentVoucherAllocation = nameof(PaymentVoucherAllocation);
    public const string Project = nameof(Project);
    public const string ProjectCorrespondence = nameof(ProjectCorrespondence);
    public const string ProjectFile = nameof(ProjectFile);
    public const string ProjectMember = nameof(ProjectMember);
    public const string ProjectStage = nameof(ProjectStage);
    public const string ProjectTask = nameof(ProjectTask);
    public const string PurchaseInvoice = nameof(PurchaseInvoice);
    public const string PurchaseInvoiceItem = nameof(PurchaseInvoiceItem);
    public const string Quotation = nameof(Quotation);
    public const string QuotationItem = nameof(QuotationItem);
    public const string QuotationPaymentTerm = nameof(QuotationPaymentTerm);
    public const string Role = nameof(Role);
    public const string Service = nameof(Service);
    public const string Supplier = nameof(Supplier);
    public const string User = nameof(User);
}
