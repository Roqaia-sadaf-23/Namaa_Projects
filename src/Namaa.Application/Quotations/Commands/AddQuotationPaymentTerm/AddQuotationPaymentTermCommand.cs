using MediatR;using Namaa.Application.Quotations.Dtos;using Namaa.Domain.Common.Results;namespace Namaa.Application.Quotations.Commands.AddQuotationPaymentTerm;public sealed record AddQuotationPaymentTermCommand(long QuotationId,decimal? Percentage,decimal? Amount,string? Description,int? DueAfterDays,DateTime? DueDate):IRequest<Result<QuotationPaymentTermDto>>;

