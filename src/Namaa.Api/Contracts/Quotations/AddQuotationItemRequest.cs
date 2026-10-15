namespace Namaa.Api.Contracts.Quotations;public sealed record AddQuotationItemRequest(string Description,decimal Quantity,decimal UnitPrice,decimal DiscountAmount,decimal TaxAmount,long? ServiceId);

