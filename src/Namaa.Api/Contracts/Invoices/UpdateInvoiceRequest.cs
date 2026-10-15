namespace Namaa.Api.Contracts.Invoices;public sealed record UpdateInvoiceRequest(DateTime IssueDate, DateTime? DueDate, string? Notes);
