namespace Namaa.Api.Contracts.Quotations;public sealed record AddQuotationPaymentTermRequest(decimal? Percentage,decimal? Amount,string? Description,int? DueAfterDays,DateTime? DueDate,string RowVersion);

