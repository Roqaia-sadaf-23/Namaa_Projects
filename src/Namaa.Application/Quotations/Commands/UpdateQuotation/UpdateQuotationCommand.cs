using MediatR;using Namaa.Application.Common.Concurrency;using Namaa.Application.Quotations.Dtos;using Namaa.Domain.Common.Results;
namespace Namaa.Application.Quotations.Commands.UpdateQuotation;
public sealed record UpdateQuotationCommand(long Id,string Title,DateTime IssueDate,DateTime? ValidUntil,string? Notes,string? Terms,string RowVersion):IRequest<Result<QuotationDto>>,IConcurrencyCommand;

