using MediatR;using Namaa.Application.Quotations.Dtos;using Namaa.Domain.Common.Results;
namespace Namaa.Application.Quotations.Commands.CreateQuotation;
public sealed record CreateQuotationCommand(long CustomerId,string QuotationNumber,string Title,DateTime IssueDate,DateTime? ValidUntil,string? Notes,string? Terms):IRequest<Result<QuotationDto>>;

