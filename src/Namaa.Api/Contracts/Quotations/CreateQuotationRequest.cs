namespace Namaa.Api.Contracts.Quotations;public sealed record CreateQuotationRequest(long CustomerId,string QuotationNumber,string Title,DateTime IssueDate,DateTime? ValidUntil,long CreatedByUserId,string? Notes,string? Terms);

