namespace Namaa.Api.Contracts.Quotations;public sealed record CreateQuotationRequest(long CustomerId,string QuotationNumber,string Title,DateTime IssueDate,DateTime? ValidUntil,string? Notes,string? Terms);

