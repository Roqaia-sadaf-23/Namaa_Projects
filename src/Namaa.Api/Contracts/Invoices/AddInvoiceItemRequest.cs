namespace Namaa.Api.Contracts.Invoices;public sealed record AddInvoiceItemRequest(string Description,decimal Quantity,decimal UnitPrice,decimal DiscountAmount,decimal TaxAmount,long? ServiceId);
