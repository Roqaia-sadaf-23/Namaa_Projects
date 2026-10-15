using MediatR;using Namaa.Application.Quotations.Dtos;using Namaa.Domain.Common.Results;namespace Namaa.Application.Quotations.Commands.AddQuotationItem;public sealed record AddQuotationItemCommand(long QuotationId,string Description,decimal Quantity,decimal UnitPrice,decimal DiscountAmount,decimal TaxAmount,long? ServiceId):IRequest<Result<QuotationItemDto>>;

