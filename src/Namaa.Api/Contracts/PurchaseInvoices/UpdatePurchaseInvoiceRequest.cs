namespace Namaa.Api.Contracts.PurchaseInvoices;public sealed record UpdatePurchaseInvoiceRequest(DateTime IssueDate, DateTime? DueDate, string? SupplierInvoiceNumber, string? Notes);
