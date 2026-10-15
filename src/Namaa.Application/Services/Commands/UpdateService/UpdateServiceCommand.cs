using MediatR;
using Namaa.Application.Services.Dtos;
using Namaa.Application.Common.Concurrency;
using Namaa.Domain.Common.Results;

namespace Namaa.Application.Services.Commands.UpdateService;

public sealed record UpdateServiceCommand(long Id, string Name, decimal DefaultPrice, string? Category, string? Description, string RowVersion) : IRequest<Result<ServiceDto>>, IConcurrencyCommand;

