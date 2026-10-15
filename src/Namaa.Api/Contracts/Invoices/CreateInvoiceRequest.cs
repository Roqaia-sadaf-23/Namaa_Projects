namespace Namaa.Api.Contracts.Invoices;public sealed record CreateInvoiceRequest(string InvoiceNumber, DateTime IssueDate, DateTime? DueDate, long? ProjectId, long? CustomerId, string? Notes);
