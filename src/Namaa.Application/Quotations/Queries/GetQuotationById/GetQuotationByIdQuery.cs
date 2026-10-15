using MediatR;using Namaa.Application.Quotations.Dtos;using Namaa.Domain.Common.Results;namespace Namaa.Application.Quotations.Queries.GetQuotationById;public sealed record GetQuotationByIdQuery(long Id):IRequest<Result<QuotationDto>>;

