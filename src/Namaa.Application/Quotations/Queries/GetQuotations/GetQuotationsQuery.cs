using MediatR;using Namaa.Application.Quotations.Dtos;using Namaa.Domain.Common.Results;namespace Namaa.Application.Quotations.Queries.GetQuotations;public sealed record GetQuotationsQuery:IRequest<Result<IReadOnlyList<QuotationDto>>>;

